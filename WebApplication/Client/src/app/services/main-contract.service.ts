import { Injectable } from '@angular/core';
import {interval} from "rxjs/observable/interval";
import {environment} from "../../environments/environment";
import {CommonService} from "./common.service";
import {BigNumber} from "bignumber.js";
import * as Web3 from "web3";
import {BehaviorSubject} from "rxjs/BehaviorSubject";
import {Subject} from "rxjs/Subject";
import {Observable} from "rxjs/Observable";
import {APIService} from "./api.service";
import {AllTokensBalance} from "../interfaces/all-tokens-balance";
import {TokenList} from "../interfaces/token-list";

@Injectable()
export class MainContractService {

  private contractAddress = environment.mainContractAddress;
  private contractABI = environment.mainContractABI;
  private fotronContractABI = environment.fotronContractABI;

  private _infuraUrl = environment.infuraUrl;
  public _contractInfura: any;
  public _contractMetamask: any;

  private _web3Infura: Web3 = null;
  private _web3Metamask: Web3 = null;
  private _lastAddress: string | null;

  private _obsEthAddressSubject = new BehaviorSubject(null);
  private _obsEthAddress = this._obsEthAddressSubject.asObservable();

  private _obsPromoBonusSubject = new BehaviorSubject(null);
  private _obsPromoBonus = this._obsPromoBonusSubject.asObservable();

  private _obsWinBIGPromoBonusSubject = new BehaviorSubject(null);
  private _obsWinBIGPromoBonus = this._obsWinBIGPromoBonusSubject.asObservable();

  private _obsWinQUICKPromoBonusSubject = new BehaviorSubject(null);
  private _obsWinQUICKPromoBonus = this._obsWinQUICKPromoBonusSubject.asObservable();

  private _obsUserTotalRewardSubject = new BehaviorSubject(null);
  private _obsUserTotalReward = this._obsUserTotalRewardSubject.asObservable();

  private destroy$: Subject<boolean> = new Subject<boolean>();
  private web3: Web3 = new Web3();

  public isRefAvailable$ = new BehaviorSubject(null);
  public passTokensBalance$ = new BehaviorSubject(null);
  public tokensBalance: AllTokensBalance[] = [];
  public prevUserTotalReward: number = null;
  public getSuccessWithdrawRequestLink$ = new Subject();

  constructor(
    private commonService: CommonService,
    private apiService: APIService
  ) {
    this.commonService.initMainContract$.subscribe(init => {
      init && this.setInterval();
    });
  }

  private checkWeb3() {
    if (!this._web3Infura && this.contractAddress) {
      this._web3Infura = new Web3(new Web3.providers.HttpProvider(this._infuraUrl));

      if (this._web3Infura['eth']) {
        this._contractInfura = this._web3Infura['eth'].contract(JSON.parse(this.contractABI)).at(this.contractAddress);
        this.initBankInfoMethods();
      } else {
        this._web3Infura = null;
      }
    }

    if (!this._web3Metamask && (window.hasOwnProperty('web3') || window.hasOwnProperty('tron')) && this.contractAddress) {
      let tron = window['tron'];

      if (tron) {
        this._web3Metamask = new Web3(tron);
        tron.enable().then();
      } else {
        this._web3Metamask = new Web3(window['web3'].currentProvider);
      }

      if (this._web3Metamask['eth']) {
        this._contractMetamask = this._web3Metamask['eth'].contract(JSON.parse(this.contractABI)).at(this.contractAddress);
      } else {
        this._web3Metamask = null;
      }
    }

    var addr = this._web3Metamask && this._web3Metamask['eth'] && this._web3Metamask['eth'].accounts.length
      ? this._web3Metamask['eth'].accounts[0] : null;

    if (this._lastAddress !== addr) {
      this._lastAddress = addr;
      window['tron'] && window['tron'].enable().then();
      this.emitAddress(addr);
    }
  }

  private setInterval() {
    interval(500).takeUntil(this.destroy$).subscribe(this.checkWeb3.bind(this));
    interval(7500).takeUntil(this.destroy$).subscribe(this.checkBalance.bind(this));
    interval(60000).takeUntil(this.destroy$).subscribe(this.updatePromoBonus.bind(this));
    interval(10000).takeUntil(this.destroy$).subscribe(this.updateWinBIGPromoBonus.bind(this));
    interval(10000).takeUntil(this.destroy$).subscribe(this.updateWinQUICKPromoBonus.bind(this));
  }

  private initBankInfoMethods() {
    this.updatePromoBonus();
    this.updateWinBIGPromoBonus();
    this.updateWinQUICKPromoBonus();
  }

  private emitAddress(ethAddress: string) {
    this._web3Metamask && this._web3Metamask['eth'] && this._web3Metamask['eth'].coinbase
    && (this._web3Metamask['eth'].defaultAccount = this._web3Metamask['eth'].coinbase);

    this._obsEthAddressSubject.next(ethAddress);
    this.checkBalance();
  }

  private checkBalance() {
    this.updateUserTotalReward();
  }

  private getAllUserBalances() {
    this.apiService.getTokenList().subscribe((tokenList: any) => {
      let count = 0;
      this.tokensBalance = [];

      tokenList.data.forEach(token => {
        let contractMetamask = this._web3Metamask['eth'].contract(JSON.parse(this.fotronContractABI)).at(token.fotronContractAddress);

        contractMetamask && contractMetamask.getCurrentUserLocalTokenBalance((err, res) => {
          const balance = +new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
          const wei = this.web3['toWei'](+balance);

          if (balance > 0) {
            contractMetamask && contractMetamask.estimateSellOrder(wei, true, (err, res) => {
              const estimate = +new BigNumber(res[0].toString()).div(new BigNumber(10).pow(18));
              this.tokensBalance.push({token: token.ticker, balance, estimate});
              count++;
              count === tokenList.data.length && this.passTokensBalance$.next(this.tokensBalance);
            });
          } else {
            this.tokensBalance.push({token: token.ticker, balance, estimate: 0});
            count++;
            count === tokenList.data.length && this.passTokensBalance$.next(this.tokensBalance);
          }
        });
      });
    });
  }

  private updatePromoBonus() {
    if (!this._contractInfura) {
      this._obsPromoBonusSubject.next(null);
    } else {
      let promoBonus = {};
      this._contractInfura && this._contractInfura._currentQuickPromoBonus((err, res) => {
        promoBonus['quick'] = new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
        this._contractInfura && this._contractInfura._currentBigPromoBonus((err, res) => {
          promoBonus['big'] = new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
          this._obsPromoBonusSubject.next(promoBonus);
        });
      });
    }
  }

  private updateWinBIGPromoBonus() {
    if (!this._contractInfura) {
      this._obsWinBIGPromoBonusSubject.next(null);
    } else {
      this._contractInfura.getBigPromoRemainingBlocks((err, res) => {
        this._obsWinBIGPromoBonusSubject.next(res);
      });
    }
  }

  private updateWinQUICKPromoBonus() {
    if (!this._contractInfura) {
      this._obsWinQUICKPromoBonusSubject.next(null);
    } else {
      this._contractInfura.getQuickPromoRemainingBlocks((err, res) => {
        this._obsWinQUICKPromoBonusSubject.next(res);
      });
    }
  }

  private updateUserTotalReward() {
    if (!this._contractMetamask || this._lastAddress === null) {
      this._obsUserTotalRewardSubject.next(null);
    } else {
      this._contractMetamask.getCurrentUserTotalReward((err, res) => {
        let result = +new BigNumber(res.toString()).div(new BigNumber(10).pow(18));

        this.prevUserTotalReward !== result && this.getAllUserBalances();
        this.prevUserTotalReward = result;
        this._obsUserTotalRewardSubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
      });
    }
  }

  public getObservableEthAddress(): Observable<string> {
    return this._obsEthAddress;
  }

  public getObservablePromoBonus(): Observable<any> {
    return this._obsPromoBonus;
  }

  public getObservableWinBIGPromoBonus(): Observable<any> {
    return this._obsWinBIGPromoBonus;
  }

  public getObservableWinQUICKPromoBonus(): Observable<any> {
    return this._obsWinQUICKPromoBonus;
  }

  public getObservableUserTotalReward(): Observable<any> {
    return this._obsUserTotalReward;
  }

  public withdraw() {
    this.apiService.getTokenList().subscribe((data: any) => {
      let tokenList: TokenList = data.data;
      let contractInfura = this._web3Infura['eth'].contract(JSON.parse(this.fotronContractABI)).at(tokenList[0].fotronContractAddress);

      contractInfura && contractInfura.getMaxGasPrice((err, res) => {
        if (+res) {
          this._contractMetamask.withdrawUserReward({ gas: 600000, gasPrice: +res }, (err, res) => {
            this.getSuccessWithdrawRequestLink$.next(res);
          });
        }
      });
    });
  }

}

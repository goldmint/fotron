import { Injectable } from '@angular/core';
import {interval} from "rxjs/observable/interval";
import {environment} from "../../environments/environment";
import {CommonService} from "./common.service";
import {BigNumber} from "bignumber.js";
import * as TronWeb from "tronweb";
import {BehaviorSubject} from "rxjs/BehaviorSubject";
import {Subject} from "rxjs/Subject";
import {Observable} from "rxjs/Observable";

let self;

@Injectable()
export class MainContractService {

  private privateKey = environment.privateKey;
  private tronWebMain = null;
  private tronWebBrowser = null;
  private TRX = new BigNumber("1000000");
  private lastTronAddress: string = null;
  public prevUserTotalReward: number = null;

  private fotronCoreContractAddress = environment.fotronCoreContractAddress;
  private fotronCoreContractAbi = environment.fotronCoreContractAbi;

  private fotronContractAddress = environment.fotronContractAddress;
  private fotronContractAbi = environment.fotronContractAbi;

  public fotronCoreContract = null;

  private _obsTronAddressSubject = new BehaviorSubject(null);
  private _obsTronAddress = this._obsTronAddressSubject.asObservable();

  private _obsPromoBonusSubject = new BehaviorSubject(null);
  private _obsPromoBonus = this._obsPromoBonusSubject.asObservable();

  private _obsWinBIGPromoBonusSubject = new BehaviorSubject(null);
  private _obsWinBIGPromoBonus = this._obsWinBIGPromoBonusSubject.asObservable();

  private _obsWinQUICKPromoBonusSubject = new BehaviorSubject(null);
  private _obsWinQUICKPromoBonus = this._obsWinQUICKPromoBonusSubject.asObservable();

  private _obsUserTotalRewardSubject = new BehaviorSubject(null);
  private _obsUserTotalReward = this._obsUserTotalRewardSubject.asObservable();

  private destroy$: Subject<boolean> = new Subject<boolean>();
  private providerURL = environment.providerURL;

  public isRefAvailable$ = new BehaviorSubject(null);
  public passTokensBalance$ = new BehaviorSubject(null);

  public getSuccessWithdrawRequestLink$ = new Subject();

  constructor(
    private commonService: CommonService
  ) {
    self = this;
    this.commonService.initMainContract$.subscribe(init => {
      init && this.setInterval();
    });
  }

  private setInterval() {
    interval(500).takeUntil(this.destroy$).subscribe(this.checkTronWeb.bind(this));
    interval(7500).takeUntil(this.destroy$).subscribe(this.checkBalance.bind(this));
    interval(60000).takeUntil(this.destroy$).subscribe(this.updatePromoBonus.bind(this));
    interval(10000).takeUntil(this.destroy$).subscribe(this.updateWinBIGPromoBonus.bind(this));
    interval(10000).takeUntil(this.destroy$).subscribe(this.updateWinQUICKPromoBonus.bind(this));
  }

  checkTronWeb() {
    if (!this.tronWebMain) {
      const HttpProvider = TronWeb.providers.HttpProvider;

      const fullNode = new HttpProvider(this.providerURL);
      const solidityNode = new HttpProvider(this.providerURL);
      const eventServer = this.providerURL;

      this.tronWebMain = new TronWeb(
        fullNode,
        solidityNode,
        eventServer,
        this.privateKey
      );

      (async function initContract() {
        self.fotronCoreContract = await self.tronWebMain.contract(JSON.parse(self.fotronCoreContractAbi)).at(self.fotronCoreContractAddress);
        self.initBankInfoMethods();
      })();
    }

    if (!this.tronWebBrowser && window.hasOwnProperty('tronWeb')) {
      this.tronWebBrowser = window['tronWeb'];
    }

    let address = this.tronWebBrowser && this.tronWebBrowser.defaultAddress.base58 ? this.tronWebBrowser.defaultAddress.base58 : null;

    if (this.fotronCoreContract && this.lastTronAddress !== address) {
      this.lastTronAddress = address;
      this.emitAddress(address);
    }
  }

  public convertNumResult2Trx(num) {
    return new BigNumber(num.toString(10)).div(this.TRX).toString(10);
  }

  private initBankInfoMethods() {
    this.updatePromoBonus();
    this.updateWinBIGPromoBonus();
    this.updateWinQUICKPromoBonus();
  }

  private emitAddress(address: string) {
    this.tronWebMain.setAddress(address);

    this._obsTronAddressSubject.next(address);
    this.checkBalance();
  }

  private checkBalance() {
    this.updateUserTotalReward();
  }

  // private getAllUserBalances() {
  //   this.apiService.getTokenList().subscribe((tokenList: any) => {
  //     let count = 0;
  //     this.tokensBalance = [];
  //
  //     tokenList.data.forEach(token => {
  //       let contractMetamask = this._web3Metamask['eth'].contract(JSON.parse(this.fotronContractABI)).at(token.fotronContractAddress);
  //
  //       contractMetamask && contractMetamask.getCurrentUserLocalTokenBalance((err, res) => {
  //         const balance = +new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
  //         const wei = this.web3['toWei'](+balance);
  //
  //         if (balance > 0) {
  //           contractMetamask && contractMetamask.estimateSellOrder(wei, true, (err, res) => {
  //             const estimate = +new BigNumber(res[0].toString()).div(new BigNumber(10).pow(18));
  //             this.tokensBalance.push({token: token.ticker, balance, estimate});
  //             count++;
  //             count === tokenList.data.length && this.passTokensBalance$.next(this.tokensBalance);
  //           });
  //         } else {
  //           this.tokensBalance.push({token: token.ticker, balance, estimate: 0});
  //           count++;
  //           count === tokenList.data.length && this.passTokensBalance$.next(this.tokensBalance);
  //         }
  //       });
  //     });
  //   });
  // }

  private updatePromoBonus() {
    if (!this.fotronCoreContract) {
      this._obsPromoBonusSubject.next(null);
    } else {
      (async function init() {
        let promoBonus = {};
        promoBonus['quick'] = +await self.fotronCoreContract._currentQuickPromoBonus().call();
        promoBonus['big'] = +await self.fotronCoreContract._currentBigPromoBonus().call();
        self._obsPromoBonusSubject.next(promoBonus);
      })();
    }
  }

  private updateWinBIGPromoBonus() {
    if (!this.fotronCoreContract) {
      this._obsWinBIGPromoBonusSubject.next(null);
    } else {
      (async function init() {
        let res = +await self.fotronCoreContract.getBigPromoRemainingBlocks().call();
        self._obsWinBIGPromoBonusSubject.next(res);
      })();
    }
  }

  private updateWinQUICKPromoBonus() {
    if (!this.fotronCoreContract) {
      this._obsWinQUICKPromoBonusSubject.next(null);
    } else {
      (async function init() {
        let res = +await self.fotronCoreContract.getQuickPromoRemainingBlocks().call();
        self._obsWinQUICKPromoBonusSubject.next(res);
      })();
    }
  }

  private updateUserTotalReward() {
    if (!this.fotronCoreContract) {
      this._obsUserTotalRewardSubject.next(null);
    } else {
      (async function init() {
        let res = +await self.fotronCoreContract.getCurrentUserTotalReward().call();

        // self.prevUserTotalReward !== +res && self.getAllUserBalances();
        self.prevUserTotalReward = res;
        self._obsUserTotalRewardSubject.next(res);
        console.log('updateUserTotalReward', res);
      })();
    }
  }

  public getObservableTronAddress(): Observable<string> {
    return this._obsTronAddress;
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
    let self = this,
        hash;
    (async function initContract() {
      self.fotronCoreContract = await self.tronWebMain.contract(JSON.parse(self.fotronContractAbi)).at(self.fotronContractAddress);
      self.fotronCoreContract && (hash = await self.fotronCoreContract.withdrawUserReward().call());
      hash && self.getSuccessWithdrawRequestLink$.next(hash);
    })();
  }
}

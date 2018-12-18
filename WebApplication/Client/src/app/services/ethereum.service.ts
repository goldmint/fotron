import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { interval } from "rxjs/observable/interval";
import * as Web3 from "web3";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { BigNumber } from 'bignumber.js'
import {environment} from "../../environments/environment";
import {Subject} from "rxjs/Subject";
import {MarketData} from "../interfaces/market-data";
import {CommonService} from "./common.service";

@Injectable()
export class TronService {

  private _infuraUrl = environment.infuraUrl;

  private fotronContractAddress;
  private fotronContractABI = environment.fotronContractABI;

  private tokenContractAddress;
  private tokenContractABI = environment.tokenABI;

  private _web3Infura: Web3 = null;
  private _web3Metamask: Web3 = null;
  private _lastAddress: string | null;
  private _metamaskNetwork: number = null;

  public _contractInfura: any;
  public _contractMetamask: any;
  public _contractMntp: any;

  private _obsEthAddressSubject = new BehaviorSubject(null);
  private _obsEthAddress = this._obsEthAddressSubject.asObservable();

  private _obsTokenBalanceSubject = new BehaviorSubject(null);
  private _obsTokenBalance = this._obsTokenBalanceSubject.asObservable();

  private _obsEthBalanceSubject = new BehaviorSubject(null);
  private _obsEthBalance = this._obsEthBalanceSubject.asObservable();

  private _obs1TokenPriceSubject = new BehaviorSubject(null);
  private _obs1TokenPrice = this._obs1TokenPriceSubject.asObservable();

  private _obsUserRewardSubject = new BehaviorSubject(null);
  private _obsUserReward = this._obsUserRewardSubject.asObservable();

  private _obsTotalDataSubject = new BehaviorSubject(null);
  private _obsTotalData = this._obsTotalDataSubject.asObservable();

  private _obsPromoBonusSubject = new BehaviorSubject(null);
  private _obsPromoBonus = this._obsPromoBonusSubject.asObservable();

  private _obsWinBIGPromoBonusSubject = new BehaviorSubject(null);
  private _obsWinBIGPromoBonus = this._obsWinBIGPromoBonusSubject.asObservable();

  private _obsWinQUICKPromoBonusSubject = new BehaviorSubject(null);
  private _obsWinQUICKPromoBonus = this._obsWinQUICKPromoBonusSubject.asObservable();

  private _obsTotalTokenSupplySubject = new BehaviorSubject(null);
  private _obsTotalTokenSupply = this._obsTotalTokenSupplySubject.asObservable();

  private _obsExpirationTimeSubject = new BehaviorSubject(null);
  private _obsExpirationTime = this._obsExpirationTimeSubject.asObservable();

  private _obsNetworkSubject = new BehaviorSubject<Number>(null);
  private _obsNetwork: Observable<Number> = this._obsNetworkSubject.asObservable();

  private _obsTokenDealRangeSubject = new BehaviorSubject<any>(null);
  private _obsTokenDealRange: Observable<Number> = this._obsTokenDealRangeSubject.asObservable();

  private _obsEthDealRangeSubject = new BehaviorSubject<any>(null);
  private _obsEthDealRange: Observable<Number> = this._obsEthDealRangeSubject.asObservable();

  public passEthBalance = new BehaviorSubject(null);
  public passTokenBalance = new BehaviorSubject(null);
  public passEthAddress = new BehaviorSubject(null);

  public getSuccessBuyRequestLink$ = new Subject();
  public getSuccessSellRequestLink$ = new Subject();

  private isTradePage: boolean;
  private destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private commonService: CommonService
  ) {
    this.commonService.passMarketData$.subscribe((data: MarketData) => {
      if (data) {
        this.stopService();
        this.fotronContractAddress = data.fotronContractAddress;
        this.tokenContractAddress = data.tokenContractAddress;
        this.isTradePage = data.isTradePage;
        this.setInterval();
      } else {
        this.stopService();
      }
    });
  }

  private stopService() {
    this._lastAddress = this._web3Infura = this._web3Metamask = null;
    this.destroy$.next(true);
  }

  private setInterval() {
    interval(500).takeUntil(this.destroy$).subscribe(this.checkWeb3.bind(this));
    interval(7500).takeUntil(this.destroy$).subscribe(this.checkBalance.bind(this));
    interval(60000).takeUntil(this.destroy$).subscribe(this.checkContractData.bind(this));
    this.isTradePage && interval(10000).takeUntil(this.destroy$).subscribe(this.updateWinBIGPromoBonus.bind(this));
    this.isTradePage && interval(10000).takeUntil(this.destroy$).subscribe(this.updateWinQUICKPromoBonus.bind(this));
  }

  private checkWeb3() {
    if (!this._web3Infura && this.fotronContractAddress) {
      this._web3Infura = new Web3(new Web3.providers.HttpProvider(this._infuraUrl));

      if (this._web3Infura['eth']) {
        this._contractInfura = this._web3Infura['eth'].contract(JSON.parse(this.fotronContractABI)).at(this.fotronContractAddress);
        this.isTradePage && this.initMethodsFromInfura();
        this.update1TokenPrice();
      } else {
        this._web3Infura = null;
      }
    }

    if (!this._web3Metamask && (window.hasOwnProperty('web3') || window.hasOwnProperty('tron')) && this.fotronContractAddress) {
      let tron = window['tron'];

      if (tron) {
        this._web3Metamask = new Web3(tron);
        tron.enable().then();
      } else {
        this._web3Metamask = new Web3(window['web3'].currentProvider);
      }

      if (this._web3Metamask['eth']) {
        this._contractMetamask = this._web3Metamask['eth'].contract(JSON.parse(this.fotronContractABI)).at(this.fotronContractAddress);
        this._contractMntp = this._web3Metamask.eth.contract(JSON.parse(this.tokenContractABI)).at(this.tokenContractAddress);
      } else {
        this._web3Metamask = null;
      }
    }

    if (this._web3Metamask && this._web3Metamask.version.network !== this._metamaskNetwork) {
      this._metamaskNetwork && this.checkBalance();

      this._metamaskNetwork = this._web3Metamask.version.network;
      this._obsNetworkSubject.next(this._metamaskNetwork);
    }

    var addr = this._web3Metamask && this._web3Metamask['eth'] && this._web3Metamask['eth'].accounts.length
      ? this._web3Metamask['eth'].accounts[0] : null;

    if (this._lastAddress !== addr) {
      this._lastAddress = addr;
      window['tron'] && window['tron'].enable().then();
      this.emitAddress(addr);
    }
  }

  private initMethodsFromInfura() {
    this.updatePromoBonus();
    this.updateWinBIGPromoBonus();
    this.updateWinQUICKPromoBonus();
    this.updateTotalData();
    this.updateTotalTokenSupply();
    this.getExpirationTime();
  }

  private checkBalance() {
    if (this._lastAddress != null) {
      this.updateTokenBalance(this._lastAddress);
      this.updateUserReward();
    }
    this.updateEthBalance(this._lastAddress);
  }

  private checkContractData() {
    this.update1TokenPrice();

    if (this.isTradePage) {
      this.updateTotalData();
      this.updatePromoBonus();
      this.updateTotalTokenSupply();
    }
  }

  private emitAddress(ethAddress: string) {
    this._web3Metamask && this._web3Metamask['eth'] && this._web3Metamask['eth'].coinbase
          && (this._web3Metamask['eth'].defaultAccount = this._web3Metamask['eth'].coinbase);

    this._obsEthAddressSubject.next(ethAddress);
    this.checkBalance();
    this.getTokenDealRange();
    this.getEthDealRange();
  }

  private updateTokenBalance(addr: string) {
    if (addr == null || this._contractMetamask == null) {
      this._obsTokenBalanceSubject.next(null);
    } else {
      this._contractMetamask.getCurrentUserLocalTokenBalance((err, res) => {
        this._obsTokenBalanceSubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
      });
    }
  }

  private updateEthBalance(addr: string) {
    if (addr == null || this._contractMetamask == null) {
      this._obsEthBalanceSubject.next(null);
    } else {
      this._contractMetamask._eth.getBalance(addr, (err, res) => {
        this._obsEthBalanceSubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
      });
    }
  }

  private update1TokenPrice() {
    if (!this._contractInfura) {
      this._obs1TokenPriceSubject.next(null);
    } else {
      let price = {};
      this._contractInfura.get1TokenBuyPrice((err, res) => {
        price['buy'] = new BigNumber(res.toString()).div(new BigNumber(10).pow(18));

        this._contractInfura.get1TokenSellPrice((err, res) => {
          price['sell'] = new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
          this._obs1TokenPriceSubject.next(price);
        });
      });
    }
  }

  private updateUserReward() {
    if (!this._contractMetamask || this._lastAddress === null) {
      this._obsUserRewardSubject.next(null);
    } else {
      this._contractMetamask.getCurrentUserReward(true, true, (err, res) => {
        this._obsUserRewardSubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
      });
    }
  }

  private updateTotalData() {
    if (!this._contractInfura) {
      this._obsTotalDataSubject.next(null);
    } else {
      let total = {};
      this._contractInfura.getTotalTokenSold((err, res) => {
        total['tokens'] = new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
        this._contractInfura.getTotalEthBalance((err, res) => {
          total['eth'] = new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
          this._obsTotalDataSubject.next(total);
        });
      });
    }
  }

  private updatePromoBonus() {
    if (!this._contractInfura) {
      this._obsPromoBonusSubject.next(null);
    } else {
      let promoBonus = {};
      this._contractInfura.getCurrentQuickPromoBonus((err, res) => {
        promoBonus['quick'] = new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
        this._contractInfura.getCurrentBigPromoBonus((err, res) => {
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

  private updateTotalTokenSupply() {
    if (!this._contractInfura) {
      this._obsTotalTokenSupplySubject.next(null);
    } else {
      this._contractInfura.getTotalTokenSupply((err, res) => {
        this._obsTotalTokenSupplySubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
      });
    }
  }

  private getExpirationTime() {
    if (!this._contractInfura) {
      this._obsExpirationTimeSubject.next(null);
    } else {
      this._contractInfura.getExpirationTime((err, res) => {
        let time: any = {};
        time.expiration = +res;
        this._contractInfura.getRemainingTimeTillExpiration((err, res) => {
          time.tillExpiration = +res;
          this._obsExpirationTimeSubject.next(time);
        });
      });
    }
  }

  private getTokenDealRange() {
    if (!this._contractInfura) {
      this._obsTokenDealRangeSubject.next(null);
    } else {
      this._contractInfura.getTokenDealRange((err, res) => {
        if (res) {
          let limit = {
            min: +new BigNumber(res[0].toString()).div(new BigNumber(10).pow(18)),
            max: +new BigNumber(res[1].toString()).div(new BigNumber(10).pow(18))
          }
          this._obsTokenDealRangeSubject.next(limit);
        }
      });
    }
  }

  private getEthDealRange() {
    if (!this._contractInfura) {
      this._obsEthDealRangeSubject.next(null);
    } else {
      this._contractInfura.getEthDealRange((err, res) => {
        if (res) {
          let limit = {
            min: +new BigNumber(res[0].toString()).div(new BigNumber(10).pow(18)),
            max: +new BigNumber(res[1].toString()).div(new BigNumber(10).pow(18))
          }
          this._obsEthDealRangeSubject.next(limit);
        }
      });
    }
  }

  public isValidAddress(addr: string): boolean {
    return (new Web3()).isAddress(addr);
  }

  public getObservableEthAddress(): Observable<string> {
    return this._obsEthAddress;
  }

  public getObservableTokenBalance(): Observable<BigNumber> {
    return this._obsTokenBalance;
  }

  public getObservableEthBalance(): Observable<BigNumber> {
    return this._obsEthBalance;
  }

  public getObservable1TokenPrice(): Observable<any> {
    return this._obs1TokenPrice;
  }

  public getObservableUserReward(): Observable<any> {
    return this._obsUserReward;
  }

  public getObservableTotalData(): Observable<any> {
    return this._obsTotalData;
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

  public getObservableTotalTokenSupply(): Observable<any> {
    return this._obsTotalTokenSupply;
  }

  public getObservableExpirationTime(): Observable<any> {
    return this._obsExpirationTime;
  }

  public getObservableEthDealRange(): Observable<any> {
    return this._obsEthDealRange;
  }

  public getObservableTokenDealRange(): Observable<any> {
    return this._obsTokenDealRange;
  }

  public getObservableNetwork(): Observable<Number> {
    return this._obsNetwork;
  }

  public buy(refAddress: string, fromAddr: string, amount: string, minReturn: string, gasPrice: number) {
    this._contractMetamask.buy(refAddress, minReturn, { from: fromAddr, value: amount, gas: 600000, gasPrice: gasPrice }, (err, res) => {
      this.getSuccessBuyRequestLink$.next(res);
    });
  }

  public sell(fromAddr: string, amount: string, minReturn: string, gasPrice: number) {
    this._contractMntp.approve(this.fotronContractAddress, amount, { from: fromAddr, value: 0, gas: 600000, gasPrice: gasPrice }, (err, res) => {
      res && setTimeout(() => {
        this._contractMetamask.sell(amount, minReturn, { from: fromAddr, value: 0, gas: 600000, gasPrice: gasPrice }, (err, res) => {
          this.getSuccessSellRequestLink$.next(res);
        });
      }, 1000);
    });
  }
}

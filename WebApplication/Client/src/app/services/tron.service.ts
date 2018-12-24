import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { interval } from "rxjs/observable/interval";
import * as TronWeb from "tronweb";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { BigNumber } from 'bignumber.js'
import {environment} from "../../environments/environment";
import {Subject} from "rxjs/Subject";
import {MarketData} from "../interfaces/market-data";
import {CommonService} from "./common.service";

let self;

@Injectable()
export class TronService {

  private privateKey = environment.privateKey;
  private tronWebMain = null;
  private tronWebBrowser = null;
  private TRX = new BigNumber("1000000");
  private lastTronAddress: string = null;
  private providerURL = environment.providerURL;

  private tokenContractAddress: string;
  private tokenContractAbi = environment.tokenContractAbi;

  private fotronContractAddress: string;
  private fotronContractAbi = environment.fotronContractAbi;

  public tokenContract = null;
  public fotronContract = null;
  public fotronContractLocal = null;
  public tokenContractLocal = null;

  private _obsTronAddressSubject = new BehaviorSubject(null);
  private _obsTronAddress = this._obsTronAddressSubject.asObservable();

  private _obsTokenBalanceSubject = new BehaviorSubject(null);
  private _obsTokenBalance = this._obsTokenBalanceSubject.asObservable();

  private _obsTrxBalanceSubject = new BehaviorSubject(null);
  private _obsTrxBalance = this._obsTrxBalanceSubject.asObservable();

  private _obs1TokenPriceSubject = new BehaviorSubject(null);
  private _obs1TokenPrice = this._obs1TokenPriceSubject.asObservable();

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

  private _obsTrxDealRangeSubject = new BehaviorSubject<any>(null);
  private _obsTrxDealRange: Observable<Number> = this._obsTrxDealRangeSubject.asObservable();

  public passTrxBalance = new BehaviorSubject(null);
  public passTokenBalance = new BehaviorSubject(null);
  public passTrxAddress = new BehaviorSubject(null);

  public getSuccessBuyRequestLink$ = new Subject();
  public getSuccessSellRequestLink$ = new Subject();

  private isTradePage: boolean;
  private destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private commonService: CommonService
  ) {
    self = this;
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
    this.lastTronAddress = this.tronWebMain = this.tronWebBrowser = null;
    this.destroy$.next(true);
  }

  private setInterval() {
    interval(500).takeUntil(this.destroy$).subscribe(this.checkTronWeb.bind(this));
    interval(7500).takeUntil(this.destroy$).subscribe(this.checkBalance.bind(this));
    interval(60000).takeUntil(this.destroy$).subscribe(this.checkContractData.bind(this));
    this.isTradePage && interval(10000).takeUntil(this.destroy$).subscribe(this.updateWinBIGPromoBonus.bind(this));
    this.isTradePage && interval(10000).takeUntil(this.destroy$).subscribe(this.updateWinQUICKPromoBonus.bind(this));
  }

  checkTronWeb() {
    if (!this.tronWebMain) {
      const HttpProvider = TronWeb.providers.HttpProvider;

      const fullNode = new HttpProvider(this.providerURL);
      const solidityNode = new HttpProvider(this.providerURL);
      const eventServer = this.providerURL+'/';

      this.tronWebMain = new TronWeb(
        fullNode,
        solidityNode,
        eventServer,
        this.privateKey
      );

      (async function initContract() {
        self.fotronContract = await self.tronWebMain.contract(JSON.parse(self.fotronContractAbi)).at(self.fotronContractAddress);
        self.tokenContract = await self.tronWebMain.contract(JSON.parse(self.tokenContractAbi)).at(self.tokenContractAddress);
        self.isTradePage && self.initMethodsFromMain();
        self.update1TokenPrice();
      })();
    }

    if (!this.tronWebBrowser && window.hasOwnProperty('tronWeb')) {
      this.tronWebBrowser = window['tronWeb'];

      (async function initContract() {
        self.fotronContractLocal = await self.tronWebBrowser.contract(JSON.parse(self.fotronContractAbi)).at(self.fotronContractAddress);
        self.tokenContractLocal = await self.tronWebBrowser.contract(JSON.parse(self.tokenContractAbi)).at(self.tokenContractAddress);
      })();
    }

    let address = this.tronWebBrowser && this.tronWebBrowser.defaultAddress.base58 ? this.tronWebBrowser.defaultAddress.base58 : null;

    if (this.fotronContract && this.lastTronAddress !== address) {
      this.lastTronAddress = address;
      this.emitAddress(address);
    }
  }

  private convertNumResult2Trx(num) {
    return +new BigNumber(num.toString(10)).div(this.TRX).toString(10);
  }

  private initMethodsFromMain() {
    this.updatePromoBonus();
    this.updateWinBIGPromoBonus();
    this.updateWinQUICKPromoBonus();
    this.updateTotalData();
    this.updateTotalTokenSupply();
    this.getExpirationTime();
  }

  private checkBalance() {
    if (this.lastTronAddress != null) {
      this.updateTokenBalance(this.lastTronAddress);
    }
    this.updateTrxBalance(this.lastTronAddress);
  }

  private checkContractData() {
    this.update1TokenPrice();

    if (this.isTradePage) {
      this.updateTotalData();
      this.updatePromoBonus();
      this.updateTotalTokenSupply();
    }
  }

  private emitAddress(address: string) {
    this.tronWebMain.setAddress(address);
    this.tronWebBrowser.setAddress(address);

    this._obsTronAddressSubject.next(address);
    this.checkBalance();
    this.getTokenDealRange();
    this.getTrxDealRange();
  }

  private updateTokenBalance(addr: string) {
    if (addr == null || this.fotronContract == null) {
      this._obsTokenBalanceSubject.next(null);
    } else {
      (async function init() {
        let res = self.convertNumResult2Trx(+await self.fotronContract.getCurrentUserLocalTokenBalance().call());
        self._obsTokenBalanceSubject.next(res);
        console.log('updateTokenBalance', res);
      })();
    }
  }

  private updateTrxBalance(address: string) {
    if (address == null || this.fotronContract == null) {
      this._obsTrxBalanceSubject.next(null);
    } else {
      this.tronWebBrowser.trx.getBalance(address).then(balance => {
        this._obsTrxBalanceSubject.next(this.convertNumResult2Trx(balance));
        console.warn('updateTrxBalance', this.convertNumResult2Trx(balance));
      }).catch();
    }
  }

  private update1TokenPrice() {
    if (!this.fotronContract) {
      this._obs1TokenPriceSubject.next(null);
    } else {
      let price = {};
      (async function init() {
        price['buy'] = self.convertNumResult2Trx(+await self.fotronContract.get1TokenBuyPrice().call());
        price['sell'] = self.convertNumResult2Trx(+await self.fotronContract.get1TokenSellPrice().call());
        self._obs1TokenPriceSubject.next(price);
        console.log('update1TokenPrice', price);
      })();
    }
  }

  private updateTotalData() {
    if (!this.fotronContract) {
      this._obsTotalDataSubject.next(null);
    } else {
      let total = {};
      (async function init() {
        total['tokens'] = self.convertNumResult2Trx(+await self.fotronContract.getTotalTokenSold().call());
        total['trx'] = self.convertNumResult2Trx(+await self.fotronContract.getTotalTrxBalance().call());
        self._obsTotalDataSubject.next(total);
        console.log('updateTotalData', total);
      })();
    }
  }

  private updatePromoBonus() {
    if (!this.fotronContract) {
      this._obsPromoBonusSubject.next(null);
    } else {
      let promoBonus = {};
      (async function init() {
        promoBonus['quick'] = self.convertNumResult2Trx(+await self.fotronContract.getCurrentQuickPromoBonus().call());
        promoBonus['big'] = self.convertNumResult2Trx(+await self.fotronContract.getCurrentBigPromoBonus().call());
        self._obsPromoBonusSubject.next(promoBonus);
        console.log('updatePromoBonus', promoBonus);
      })();
    }
  }

  private updateWinBIGPromoBonus() {
    if (!this.fotronContract) {
      this._obsWinBIGPromoBonusSubject.next(null);
    } else {
      (async function init() {
        let res = +await self.fotronContract.getBigPromoRemainingBlocks().call();
        self._obsWinBIGPromoBonusSubject.next(res);
        console.log('updateWinBIGPromoBonus', res);
      })();
    }
  }

  private updateWinQUICKPromoBonus() {
    if (!this.fotronContract) {
      this._obsWinQUICKPromoBonusSubject.next(null);
    } else {
      (async function init() {
        let res = +await self.fotronContract.getQuickPromoRemainingBlocks().call();
        self._obsWinQUICKPromoBonusSubject.next(res);
        console.log('updateWinQUICKPromoBonus', res);
      })();
    }
  }

  private updateTotalTokenSupply() {
    if (!this.fotronContract) {
      this._obsTotalTokenSupplySubject.next(null);
    } else {
      (async function init() {
        let res = self.convertNumResult2Trx(+await self.fotronContract.getTotalTokenSupply().call());
        self._obsTotalTokenSupplySubject.next(res);
        console.log('updateTotalTokenSupply', res);
      })();
    }
  }

  private getExpirationTime() {
    if (!this.fotronContract) {
      this._obsExpirationTimeSubject.next(null);
    } else {
      (async function init() {
        let time: any = {};
        time.expiration = +await self.fotronContract.getExpirationTime().call();
        time.tillExpiration = +await self.fotronContract.getRemainingTimeTillExpiration().call();
        self._obsExpirationTimeSubject.next(time);
        console.log('getExpirationTime', time);
      })();
    }
  }

  private getTokenDealRange() {
    if (!this.fotronContract) {
      this._obsTokenDealRangeSubject.next(null);
    } else {
      (async function init() {
        let res = await self.fotronContract.getTokenDealRange().call();
        let limit = {
          min: self.convertNumResult2Trx(+res[0]),
          max: self.convertNumResult2Trx(+res[1])
        }
        self._obsTokenDealRangeSubject.next(limit);
        console.log('getTokenDealRange', limit);
      })();
    }
  }

  private getTrxDealRange() {
    if (!this.fotronContract) {
      this._obsTrxDealRangeSubject.next(null);
    } else {
      (async function init() {
        let res = await self.fotronContract.getTrxDealRange().call();
        let limit = {
          min: self.convertNumResult2Trx(+res[0]),
          max: self.convertNumResult2Trx(+res[1])
        }
        self._obsTrxDealRangeSubject.next(limit);
        console.log('getTrxDealRange', limit);
      })();
    }
  }

  public isValidAddress(address: string): boolean {
    return TronWeb['isAddress'](address);
  }

  public getObservableTronAddress(): Observable<any> {
    return this._obsTronAddress;
  }

  public getObservableTokenBalance(): Observable<any> {
    return this._obsTokenBalance;
  }

  public getObservableTrxBalance(): Observable<any> {
    return this._obsTrxBalance;
  }

  public getObservable1TokenPrice(): Observable<any> {
    return this._obs1TokenPrice;
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

  public getObservableTrxDealRange(): Observable<any> {
    return this._obsTrxDealRange;
  }

  public getObservableTokenDealRange(): Observable<any> {
    return this._obsTokenDealRange;
  }

  public getObservableNetwork(): Observable<any> {
    return this._obsNetwork;
  }

  public buy(fromAddr: string, amount: number, minReturn: number) {
    (async function buy() {
      let res = await self.fotronContractLocal.buy(fromAddr, minReturn).send({ callValue: amount });
      res && self.getSuccessBuyRequestLink$.next(res);
      console.log(res);
    })();
  }

  public sell(fromAddr: string, amount: number, minReturn: number) {
    (async function sell() {
      let res, res2;
      res = await self.tokenContractLocal.approve(fromAddr, amount).send();
      res && (res2 = await self.fotronContractLocal.sell(amount, minReturn).send());
      res2 && self.getSuccessSellRequestLink$.next(res);
      console.log(res2);
    })();
  }
}

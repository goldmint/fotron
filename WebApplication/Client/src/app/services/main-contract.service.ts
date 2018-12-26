import { Injectable } from '@angular/core';
import {interval} from "rxjs/observable/interval";
import {environment} from "../../environments/environment";
import {CommonService} from "./common.service";
import {BigNumber} from "bignumber.js";
import * as TronWeb from "tronweb";
import {BehaviorSubject} from "rxjs/BehaviorSubject";
import {Subject} from "rxjs/Subject";
import {Observable} from "rxjs/Observable";
import {APIService} from "./api.service";
import {AllTokensBalance} from "../interfaces/all-tokens-balance";
import {TronService} from "./tron.service";

let self;

@Injectable()
export class MainContractService {

  private privateKey = environment.privateKey;
  private tronWebMain = null;
  private tronWebBrowser = null;
  private TRX = new BigNumber("1000000");
  private lastTronAddress: string = null;

  private fotronCoreContractAddress = environment.fotronCoreContractAddress;
  private fotronCoreContractAbi = environment.fotronCoreContractAbi;
  private fotronContractAbi = environment.fotronContractAbi;

  public fotronCoreContract = null;
  public fotronCoreContractLocal = null;

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

  public tokensBalance: AllTokensBalance[] = [];
  public isRefAvailable$ = new BehaviorSubject(null);
  public passTokensBalance$ = new BehaviorSubject(null);

  public getSuccessWithdrawRequestLink$ = new Subject();

  constructor(
    private tronService: TronService,
    private commonService: CommonService,
    private apiService: APIService
  ) {
    self = this;
    this.commonService.initMainContract$.subscribe(init => {
      init && this.setInterval();
    });

    let tokenBalance;
    this.tronService.passTokenBalance.takeUntil(this.destroy$).subscribe(balance => {
      if (balance !== null && balance !== tokenBalance) {
        this.getAllUserBalances();
        tokenBalance = balance
      }
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
      (async function initContract() {
        self.fotronCoreContractLocal = await self.tronWebBrowser.contract(JSON.parse(self.fotronCoreContractAbi)).at(self.fotronCoreContractAddress);
      })();
    }

    let address = this.tronWebBrowser && this.tronWebBrowser.defaultAddress.base58 ? this.tronWebBrowser.defaultAddress.base58 : null;

    if (this.fotronCoreContract && this.lastTronAddress !== address) {
      this.lastTronAddress = address;
      this.emitAddress(address);
    }
  }

  public convertNumResult2Trx(num) {
    return +new BigNumber(num.toString(10)).div(this.TRX).toString(10);
  }

  private initBankInfoMethods() {
    this.updatePromoBonus();
    this.updateWinBIGPromoBonus();
    this.updateWinQUICKPromoBonus();
  }

  private emitAddress(address: string) {
    this.tronWebMain.setAddress(address);
    this.tronWebBrowser.setAddress(address);

    if (this.lastTronAddress && address) {
      (async function initContract() {
        self.fotronCoreContractLocal = await self.tronWebBrowser.contract(JSON.parse(self.fotronCoreContractAbi)).at(self.fotronCoreContractAddress);
        self.checkBalance();
      })();
    } else {
      this.checkBalance();
    }

    this._obsTronAddressSubject.next(address);
  }

  private checkBalance() {
    this.updateUserTotalReward();
  }

  private getAllUserBalances() {
    this.lastTronAddress && this.apiService.getTokenList().subscribe((tokenList: any) => {
      let count = 0;
      this.tokensBalance = [];

      tokenList.data.forEach(token => {
        (async function initContract() {
          let balance;
          let fotronContract = await self.tronWebBrowser.contract(JSON.parse(self.fotronContractAbi)).at(token.fotronContractAddress);
          fotronContract && (balance = self.convertNumResult2Trx(+await fotronContract.getCurrentUserLocalTokenBalance().call()));

          if (balance > 0) {
            let res;
            fotronContract && (res = await fotronContract.estimateSellOrder(balance * Math.pow(10, 6), true).call());

            const estimate = +res[0] / Math.pow(10, 6);
            self.tokensBalance.push({token: token.ticker, balance, estimate});
            count++;
            count === tokenList.data.length && self.passTokensBalance$.next(self.tokensBalance);
          } else {
            self.tokensBalance.push({token: token.ticker, balance, estimate: 0});
            count++;
            count === tokenList.data.length && self.passTokensBalance$.next(self.tokensBalance);
          }
        })();
      });
    });
  }

  private updatePromoBonus() {
    if (!this.fotronCoreContract) {
      this._obsPromoBonusSubject.next(null);
    } else {
      (async function init() {
        try {
          let promoBonus = {};
          promoBonus['quick'] = self.convertNumResult2Trx(+await self.fotronCoreContract._currentQuickPromoBonus().call());
          promoBonus['big'] = self.convertNumResult2Trx(+await self.fotronCoreContract._currentBigPromoBonus().call());
          self._obsPromoBonusSubject.next(promoBonus);
        } catch(e) { }
      })();
    }
  }

  private updateWinBIGPromoBonus() {
    if (!this.fotronCoreContract) {
      this._obsWinBIGPromoBonusSubject.next(null);
    } else {
      (async function init() {
        try {
          let res = +await self.fotronCoreContract.getBigPromoRemainingBlocks().call();
          self._obsWinBIGPromoBonusSubject.next(res);
        } catch(e) { }
      })();
    }
  }

  private updateWinQUICKPromoBonus() {
    if (!this.fotronCoreContract) {
      this._obsWinQUICKPromoBonusSubject.next(null);
    } else {
      (async function init() {
        try {
          let res = +await self.fotronCoreContract.getQuickPromoRemainingBlocks().call();
          self._obsWinQUICKPromoBonusSubject.next(res);
        } catch(e) { }
      })();
    }
  }

  private updateUserTotalReward() {
    if (!this.fotronCoreContract) {
      this._obsUserTotalRewardSubject.next(null);
    } else {
      (async function init() {
        try {
          let res = self.convertNumResult2Trx(+await self.fotronCoreContractLocal.getCurrentUserTotalReward().call());
          self._obsUserTotalRewardSubject.next(res);
        } catch(e) { }
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
    (async function withdraw() {
      let hash = await self.fotronCoreContractLocal.withdrawUserReward().send();
      hash && self.getSuccessWithdrawRequestLink$.next(hash);
    })();
  }
}

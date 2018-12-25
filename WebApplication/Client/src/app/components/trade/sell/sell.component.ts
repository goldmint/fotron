import {ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Subject} from "rxjs/Subject";
import * as Web3 from "web3";
import {BigNumber} from "bignumber.js";
import {environment} from "../../../../environments/environment";
import {TranslateService} from "@ngx-translate/core";
import {MessageBoxService} from "../../../services/message-box.service";
import {UserService} from "../../../services/user.service";
import {Observable} from "rxjs/Observable";
import {TokenInfo} from "../../../interfaces/token-info";
import {CommonService} from "../../../services/common.service";
import {TronService} from "../../../services/tron.service";
import {Subscription} from "rxjs/Subscription";

let self;

@Component({
  selector: 'app-sell',
  templateUrl: './sell.component.html',
  styleUrls: ['./sell.component.sass']
})
export class SellComponent implements OnInit, OnDestroy {

  @Input('tokenInfo') tokenInfo: TokenInfo;
  @ViewChild('mntpInput') mntpInput;
  @ViewChild('trxInput') trxInput;

  public loading: boolean = false;
  public isTyping: boolean = false;
  public mntp: number = 0;
  public trx: number = 0;
  public estimateFee: number = 0;
  public averageTokenPrice: number = 0;
  public tokenBalance: BigNumber | any = 0;
  public sellPrice: BigNumber | any = 0;
  public trxAddress: string = null;
  public errors = {
    invalidBalance: false,
    trxLimit: false,
    tokenLimit: false
  };
  public tronscanUrl = environment.tronscanUrl;
  public fromToken: boolean = true;
  public trxLimits = {
    min: 0,
    max: 0
  };
  public tokenLimits = {
    min: 0,
    max: 0
  };
  public isBalanceBetter: boolean = false;
  public minReturn: number;
  public isMinReturnError: boolean = false;
  public isMobile: boolean = false;

  private minReturnPercent = 1;
  private destroy$: Subject<boolean> = new Subject<boolean>();
  private sub1: Subscription;

  constructor(
    private tronService: TronService,
    private messageBox: MessageBoxService,
    private translate: TranslateService,
    private userService: UserService,
    private cdRef: ChangeDetectorRef,
    private commonService: CommonService
  ) {
    self = this;
  }

  ngOnInit() {
    this.mntpInput.valueChanges
      .debounceTime(500)
      .distinctUntilChanged()
      .takeUntil(this.destroy$)
      .subscribe(value => {
        if (this.fromToken && +value && !this.errors.invalidBalance && !this.errors.tokenLimit) {
          this.estimateSellOrder(this.mntp, true, false);
        }
      });

    this.trxInput.valueChanges
      .debounceTime(500)
      .distinctUntilChanged()
      .takeUntil(this.destroy$)
      .subscribe(value => {
        if (!this.fromToken && +value && !this.errors.invalidBalance && !this.errors.trxLimit) {
          this.estimateSellOrder(this.trx, false, false);
        }
      });

    this.initTransactionHashModal();

    this.tronService.passTokenBalance.takeUntil(this.destroy$).subscribe(gold => {
      if (gold !== null) {
        this.tokenBalance = gold;
        this.mntp = +this.substrValue(+gold);
        this.sub1 && this.sub1.unsubscribe();

        Observable.combineLatest(
          this.tronService.getObservableTokenDealRange(),
          this.tronService.getObservableTrxDealRange()
        ).takeUntil(this.destroy$).subscribe(limits => {
          if (limits[0] && limits[1]) {
            this.tokenLimits.min = limits[0].min;
            this.tokenLimits.max = limits[0].max;

            this.trxLimits.min = limits[1].min;
            this.trxLimits.max = limits[1].max;
          }
        });
        this.cdRef.markForCheck();
      }
    });
    this.tronService.passTrxAddress.takeUntil(this.destroy$).subscribe(address => {
      address && (this.trxAddress = address);
      if (this.trxAddress && !address) {
        this.trxAddress = address;
        this.tokenBalance = this.mntp = this.trx = 0;
      }
      this.cdRef.markForCheck();
    });

    this.tronService.getObservable1TokenPrice().takeUntil(this.destroy$).subscribe(price => {
      price && (this.sellPrice = price.sell);
      this.cdRef.markForCheck();
    });

    this.commonService.isMobile$.takeUntil(this.destroy$).subscribe(isMobile => this.isMobile = isMobile);
  }

  changeValue(event, fromToken: boolean) {
    this.isTyping = true;
    this.fromToken = fromToken;

    event.target.value = this.substrValue(event.target.value);
    fromToken ? this.mntp = +event.target.value : this.trx = +event.target.value;

    this.checkErrors(fromToken, +event.target.value);
  }

  setCoinBalance(percent) {
    if (this.trxAddress) {
      let value = this.isBalanceBetter ? this.substrValue(this.tokenLimits.max * percent) : this.substrValue(+this.tokenBalance * percent);
      if (+value != this.mntp) {
        this.mntp = +value;
      }
      this.checkErrors(true, value);
      this.cdRef.markForCheck();
    }
  }

  substrValue(value) {
    return value.toString()
      .replace(',', '.')
      .replace(/([^\d.])|(^\.)/g, '')
      .replace(/^(\d{1,6})\d*(?:(\.\d{0,6})[\d.]*)?/, '$1$2')
      .replace(/^0+(\d)/, '$1');
  }

  estimateSellOrder(amount: number, fromToken: boolean, isFirstLoad: boolean) {
    this.loading = true;
    this.isTyping = false;
    this.fromToken = fromToken;

    if (!amount && isFirstLoad) {
      this.loading = false;
      this.cdRef.markForCheck();
      return;
    }

    (async function init() {
      let res = await self.tronService.fotronContract.estimateSellOrder(amount * Math.pow(10, 6), fromToken).call();
      let estimate = +res[0] / Math.pow(10, 6);
      self.estimateFee = +res[1] / Math.pow(10, 6);
      self.averageTokenPrice = +res[2] / Math.pow(10, 6);

      isFirstLoad && (self.isBalanceBetter = self.mntp > self.tokenLimits.max);

      if (fromToken) {
        self.trx = +self.substrValue(estimate);
      } else {
        self.mntp = +self.substrValue(estimate);
        if (self.trxAddress && self.mntp > self.tokenBalance) {
          self.errors.invalidBalance = true;
        }
      }

      self.minReturn = +self.substrValue(self.trx * self.minReturnPercent);

      self.loading = self.isMinReturnError = false;
      self.cdRef.markForCheck();
    })();

    this.cdRef.markForCheck();
  }

  checkErrors(fromToken: boolean, value: number) {
    this.errors.invalidBalance = fromToken && this.trxAddress && this.mntp > this.tokenBalance;

    this.errors.tokenLimit = fromToken && this.trxAddress && value > 0 &&
      (value < this.tokenLimits.min || value > this.tokenLimits.max);

    this.errors.trxLimit = !fromToken && this.trxAddress && value > 0 &&
      (value < this.trxLimits.min || value > this.trxLimits.max);

    this.cdRef.markForCheck();
  }

  initTransactionHashModal() {
    this.tronService.getSuccessSellRequestLink$.takeUntil(this.destroy$).subscribe(hash => {
      if (hash) {
        this.translate.get('MESSAGE.SentTransaction').subscribe(phrases => {
          this.messageBox.alert(`
            <div class="text-center">
              <div class="font-weight-500 mb-2">${phrases.Heading}</div>
              <div>${phrases.Hash}</div>
              <div class="mb-2 modal-tx-hash overflow-ellipsis">${hash}</div>
              <a href="${this.tronscanUrl}${hash}" target="_blank">${phrases.Link}</a>
            </div>
          `);
        });
      }
    });
  }

  detectTronLink(heading) {
    if (window.hasOwnProperty('tronWeb')) {
      !this.trxAddress && this.userService.loginToTronLink(heading);
    } else {
      this.translate.get('MESSAGE.TronLink').subscribe(phrase => {
        this.messageBox.alert(phrase.Text, phrase.Heading);
      });
    }
  }

  onSubmit() {
    if (!this.trxAddress) {
      this.detectTronLink('HeadingSell');
      return;
    }

    const amount = this.mntp * Math.pow(10, 6);
    const minReturn = this.minReturn * Math.pow(10, 6);
    this.tronService.sell(this.trxAddress, amount, minReturn);
  }

  ngOnDestroy() {
    this.destroy$.next(true);
  }
}

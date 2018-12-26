import {ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Subject} from "rxjs/Subject";
import {MessageBoxService} from "../../../services/message-box.service";
import {TranslateService} from "@ngx-translate/core";
import {environment} from "../../../../environments/environment";
import {UserService} from "../../../services/user.service";
import {Observable} from "rxjs/Observable";
import {TokenInfo} from "../../../interfaces/token-info";
import {CommonService} from "../../../services/common.service";
import {TronService} from "../../../services/tron.service";
import {Subscription} from "rxjs/Subscription";

let self;

@Component({
  selector: 'app-buy',
  templateUrl: './buy.component.html',
  styleUrls: ['./buy.component.sass']
})
export class BuyComponent implements OnInit, OnDestroy {

  @Input('tokenInfo') tokenInfo: TokenInfo;
  @ViewChild('trxInput') trxInput;
  @ViewChild('mntpInput') mntpInput;

  public loading: boolean = false;
  public isTyping: boolean = false;
  public trx: number = 0;
  public mntp: number = 0;
  public estimateFee: number = 0;
  public averageTokenPrice: number = 0;
  public trxBalance: number = 0;
  public buyPrice: number = 0;
  public trxAddress: string = null;
  public errors = {
    invalidBalance: false,
    trxLimit: false,
    tokenLimit: false
  };
  public tronscanUrl = environment.tronscanUrl;
  public fromTrx: boolean = true;
  public isInvalidNetwork: boolean = false;
  public isBalanceBetter: boolean = false;
  public trxLimits = {
    min: 0,
    max: 0
  };
  public tokenLimits = {
    min: 0,
    max: 0
  };
  public minReturn: number;
  public isMinReturnError: boolean = false;
  public isMobile: boolean = false;

  private minReturnPercent = 1;
  private destroy$: Subject<boolean> = new Subject<boolean>();
  private trxBalanceForCheck: number;
  private timeOut: any;
  private sub1: Subscription;

  constructor(
    private tronService: TronService,
    private cdRef: ChangeDetectorRef,
    private messageBox: MessageBoxService,
    private translate: TranslateService,
    private userService: UserService,
    private commonService: CommonService
  ) {
    self = this;
  }

  ngOnInit() {
    this.trxInput.valueChanges
      .debounceTime(500)
      .distinctUntilChanged()
      .takeUntil(this.destroy$)
      .subscribe(value => {
        if (this.fromTrx && +value && !this.errors.invalidBalance && !this.errors.trxLimit) {
          this.estimateBuyOrder(this.trx, true, false);
        }
      });

    this.mntpInput.valueChanges
      .debounceTime(500)
      .distinctUntilChanged()
      .takeUntil(this.destroy$)
      .subscribe(value => {
        if (!this.fromTrx && +value && !this.errors.invalidBalance && !this.errors.tokenLimit) {
          this.estimateBuyOrder(this.mntp, false, false);
        }
      });

    this.initTransactionHashModal();

    this.tronService.passTrxBalance.takeUntil(this.destroy$).subscribe(trx => {
      this.trxBalanceForCheck = trx;
      this.sub1 && this.sub1.unsubscribe();

      if (trx !== null) {
        this.trxBalance = trx;
        this.trx = +this.substrValue(trx);

        this.sub1 = Observable.combineLatest(
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
        this.trxBalance = this.trx = this.mntp = 0;
      }
      this.cdRef.markForCheck();
    });

    this.tronService.getObservable1TokenPrice().takeUntil(this.destroy$).subscribe(price => {
      price && (this.buyPrice = price.buy);
      this.cdRef.markForCheck();
    });

    this.commonService.isMobile$.takeUntil(this.destroy$).subscribe(isMobile => this.isMobile = isMobile);
  }

  changeValue(event, fromTrx: boolean) {
    this.isTyping = true;
    this.fromTrx = fromTrx;

    event.target.value = this.substrValue(event.target.value);
    fromTrx ? this.trx = +event.target.value : this.mntp = +event.target.value;

    this.checkErrors(fromTrx, +event.target.value);
  }

  setCoinBalance(percent) {
    let value = this.substrValue(+this.trxBalance * percent);
    this.trx = +value;
    this.checkErrors(true, value);

    !this.errors.trxLimit && this.estimateBuyOrder(this.trx, true, false);
    this.cdRef.markForCheck();
  }

  substrValue(value) {
    return value.toString()
      .replace(',', '.')
      .replace(/([^\d.])|(^\.)/g, '')
      .replace(/^(\d{1,6})\d*(?:(\.\d{0,6})[\d.]*)?/, '$1$2')
      .replace(/^0+(\d)/, '$1');
  }

  estimateBuyOrder(amount: number, fromTrx: boolean, isFirstLoad: boolean) {
    this.loading = true;
    this.isTyping = false;
    this.fromTrx = fromTrx;

    if (!amount && isFirstLoad) {
      this.loading = false;
      this.cdRef.markForCheck();
      return;
    }

    (async function init() {
      let res = await self.tronService.fotronContract.estimateBuyOrder(amount * Math.pow(10, 6), fromTrx).call();
      let estimate = +res[0] / Math.pow(10, 6);
      self.estimateFee = +res[1] / Math.pow(10, 6);
      self.averageTokenPrice = +res[2] / Math.pow(10, 6);

      if (fromTrx) {
        self.mntp = +self.substrValue(estimate);
      } else {
        self.trx = +self.substrValue(estimate);
        if (self.trxAddress && self.trx > self.trxBalance) {
          self.errors.invalidBalance = true;
        }
      }

      self.minReturn = +self.substrValue(self.mntp * self.minReturnPercent);

      self.loading = self.isMinReturnError = false;
      self.cdRef.markForCheck();
    })();

    this.cdRef.markForCheck();
  }

  checkErrors(fromTrx: boolean, value: number) {
    this.errors.invalidBalance = fromTrx && this.trxAddress && this.trx > this.trxBalance;

    this.errors.trxLimit = fromTrx && this.trxAddress && value > 0 &&
      (value < this.trxLimits.min || value > this.trxLimits.max);

    this.errors.tokenLimit = !fromTrx && this.trxAddress && value > 0 &&
      (value < this.tokenLimits.min || value > this.tokenLimits.max);

    this.cdRef.markForCheck();
  }

  initTransactionHashModal() {
    this.tronService.getSuccessBuyRequestLink$.takeUntil(this.destroy$).subscribe(hash => {
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
      this.detectTronLink('HeadingBuy');
      return;
    }

    const amount = this.trx * Math.pow(10, 6);
    const minReturn = this.minReturn * Math.pow(10, 6);
    this.tronService.buy(this.trxAddress, amount, minReturn);
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    clearTimeout(this.timeOut);
  }

}

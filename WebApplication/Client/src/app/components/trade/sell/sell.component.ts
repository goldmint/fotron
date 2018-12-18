import {ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {EthereumService} from "../../../services/ethereum.service";
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

@Component({
  selector: 'app-sell',
  templateUrl: './sell.component.html',
  styleUrls: ['./sell.component.sass']
})
export class SellComponent implements OnInit, OnDestroy {

  @Input('tokenInfo') tokenInfo: TokenInfo;
  @ViewChild('mntpInput') mntpInput;
  @ViewChild('ethInput') ethInput;

  public loading: boolean = false;
  public isTyping: boolean = false;
  public mntp: number = 0;
  public eth: number = 0;
  public estimateFee: number = 0;
  public averageTokenPrice: number = 0;
  public tokenBalance: BigNumber | any = 0;
  public sellPrice: BigNumber | any = 0;
  public ethAddress: string = null;
  public errors = {
    invalidBalance: false,
    ethLimit: false,
    tokenLimit: false
  };
  public etherscanUrl = environment.etherscanUrl;
  public fromToken: boolean = true;
  public ethLimits = {
    min: 0,
    max: 0
  };
  public tokenLimits = {
    min: 0,
    max: 0
  };
  public isInvalidNetwork: boolean = false;
  public MMNetwork = environment.MMNetwork;
  public isBalanceBetter: boolean = false;
  public minReturn: number;
  public isMinReturnError: boolean = false;
  public isMobile: boolean = false;

  private minReturnPercent = 1;
  private web3: Web3 = new Web3();
  private destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private ethService: EthereumService,
    private messageBox: MessageBoxService,
    private translate: TranslateService,
    private userService: UserService,
    private cdRef: ChangeDetectorRef,
    private commonService: CommonService
  ) { }

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

    this.ethInput.valueChanges
      .debounceTime(500)
      .distinctUntilChanged()
      .takeUntil(this.destroy$)
      .subscribe(value => {
        if (!this.fromToken && +value && !this.errors.invalidBalance && !this.errors.ethLimit) {
          this.estimateSellOrder(this.eth, false, false);
        }
      });

    this.initTransactionHashModal();

    this.ethService.passTokenBalance.takeUntil(this.destroy$).subscribe(gold => {
      if (gold) {
        this.tokenBalance = gold;
        this.mntp = +this.substrValue(+gold);

        Observable.combineLatest(
          this.ethService.getObservableTokenDealRange(),
          this.ethService.getObservableEthDealRange()
        ).takeUntil(this.destroy$).subscribe(limits => {
          if (limits[0] && limits[1]) {
            this.tokenLimits.min = limits[0].min;
            this.tokenLimits.max = limits[0].max;

            this.ethLimits.min = limits[1].min;
            this.ethLimits.max = limits[1].max;
            this.estimateSellOrder(this.mntp, true, true);
          }
        });
        this.cdRef.markForCheck();
      }
    });
    this.ethService.passEthAddress.takeUntil(this.destroy$).subscribe(address => {
      address && (this.ethAddress = address);
      if (this.ethAddress && !address) {
        this.ethAddress = address;
        this.tokenBalance = this.mntp = this.eth = 0;
      }
      this.cdRef.markForCheck();
    });

    this.ethService.getObservable1TokenPrice().takeUntil(this.destroy$).subscribe(price => {
      price && (this.sellPrice = price.sell);
      this.cdRef.markForCheck();
    });

    this.ethService.getObservableNetwork().takeUntil(this.destroy$).subscribe(network => {
      network !== null && (this.isInvalidNetwork = network != this.MMNetwork.index);
      this.cdRef.markForCheck();
    });

    this.commonService.isMobile$.takeUntil(this.destroy$).subscribe(isMobile => this.isMobile = isMobile);
  }

  changeValue(event, fromToken: boolean) {
    this.isTyping = true;
    this.fromToken = fromToken;

    event.target.value = this.substrValue(event.target.value);
    fromToken ? this.mntp = +event.target.value : this.eth = +event.target.value;

    this.checkErrors(fromToken, +event.target.value);
  }

  changeMinReturn(event) {
    event.target.value = this.substrValue(event.target.value);
    this.minReturn = +event.target.value;

    this.isMinReturnError = this.minReturn > this.eth * this.minReturnPercent || this.minReturn <= 0;
    this.cdRef.markForCheck();
  }

  setCoinBalance(percent) {
    if (this.ethAddress) {
      let value = this.isBalanceBetter ? this.substrValue(this.tokenLimits.max * percent) : this.substrValue(+this.tokenBalance * percent);
      if (+value != this.mntp) {
        this.mntp = +value;
        // !this.errors.tokenLimit && this.estimateSellOrder(this.mntp, true, false);
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

    const wei = this.web3['toWei'](amount);
    this.ethService._contractInfura && this.ethService._contractInfura.estimateSellOrder(wei, fromToken, (err, res) => {
      if (res) {
        let estimate = +new BigNumber(res[0].toString()).div(new BigNumber(10).pow(18));
        this.estimateFee = +new BigNumber(res[1].toString()).div(new BigNumber(10).pow(18));
        this.averageTokenPrice = +new BigNumber(res[2].toString()).div(new BigNumber(10).pow(18));

        isFirstLoad && (this.isBalanceBetter = this.mntp > this.tokenLimits.max);

        if (fromToken) {
          this.eth = +this.substrValue(estimate);
          // this.errors.invalidBalance = false;
        } else {
          this.mntp = +this.substrValue(estimate);
          if (this.ethAddress && this.mntp > this.tokenBalance) {
            this.errors.invalidBalance = true;
          }
        }

        this.minReturn = +this.substrValue(this.eth * this.minReturnPercent);

        this.loading = this.isMinReturnError = false;
        this.cdRef.markForCheck();
      }
    });
    this.cdRef.markForCheck();
  }

  checkErrors(fromToken: boolean, value: number) {
    this.errors.invalidBalance = fromToken && this.ethAddress && this.mntp > this.tokenBalance;

    this.errors.tokenLimit = fromToken && this.ethAddress && value > 0 &&
      (value < this.tokenLimits.min || value > this.tokenLimits.max);

    this.errors.ethLimit = !fromToken && this.ethAddress && value > 0 &&
      (value < this.ethLimits.min || value > this.ethLimits.max);

    this.cdRef.markForCheck();
  }

  setMinReturn(percent: number) {
    !this.loading && (this.minReturn = +this.substrValue(this.eth * percent));
    this.cdRef.markForCheck();
  }

  initTransactionHashModal() {
    this.ethService.getSuccessSellRequestLink$.takeUntil(this.destroy$).subscribe(hash => {
      if (hash) {
        this.translate.get('MESSAGE.SentTransaction').subscribe(phrases => {
          this.messageBox.alert(`
            <div class="text-center">
              <div class="font-weight-500 mb-2">${phrases.Heading}</div>
              <div>${phrases.Hash}</div>
              <div class="mb-2 modal-tx-hash overflow-ellipsis">${hash}</div>
              <a href="${this.etherscanUrl}${hash}" target="_blank">${phrases.Link}</a>
            </div>
          `);
        });
      }
    });
  }

  detectMetaMask(heading) {
    if (window.hasOwnProperty('web3')) {
      !this.ethAddress && this.userService.loginToMM(heading);
    } else {
      this.translate.get('MESSAGE.MetaMask').subscribe(phrase => {
        this.messageBox.alert(phrase.Text, phrase.Heading);
      });
    }
  }

  onSubmit() {
    if (!this.ethAddress) {
      this.detectMetaMask('HeadingSell');
      return;
    }

    this.ethService._contractInfura.getMaxGasPrice((err, res) => {
      if (+res) {
        const amount = this.web3['toWei'](this.mntp);
        const minReturn = this.web3['toWei'](this.minReturn);
        this.ethService.sell(this.ethAddress, amount, minReturn, +res);
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next(true);
  }

}

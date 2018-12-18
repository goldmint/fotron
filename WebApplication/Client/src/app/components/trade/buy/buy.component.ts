import {ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {EthereumService} from "../../../services/ethereum.service";
import {Subject} from "rxjs/Subject";
import * as Web3 from "web3";
import {BigNumber} from "bignumber.js";
import {MessageBoxService} from "../../../services/message-box.service";
import {TranslateService} from "@ngx-translate/core";
import {environment} from "../../../../environments/environment";
import {UserService} from "../../../services/user.service";
import {Observable} from "rxjs/Observable";
import {TokenInfo} from "../../../interfaces/token-info";
import {CommonService} from "../../../services/common.service";

@Component({
  selector: 'app-buy',
  templateUrl: './buy.component.html',
  styleUrls: ['./buy.component.sass']
})
export class BuyComponent implements OnInit, OnDestroy {

  @Input('tokenInfo') tokenInfo: TokenInfo;
  @ViewChild('ethInput') ethInput;
  @ViewChild('mntpInput') mntpInput;

  public loading: boolean = false;
  public isTyping: boolean = false;
  public eth: number = 0;
  public mntp: number = 0;
  public estimateFee: number = 0;
  public averageTokenPrice: number = 0;
  public ethBalance: BigNumber | any = 0;
  public buyPrice: BigNumber | any = 0;
  public ethAddress: string = null;
  public errors = {
    invalidBalance: false,
    ethLimit: false,
    tokenLimit: false
  };
  public etherscanUrl = environment.etherscanUrl;
  public fromEth: boolean = true;
  public isInvalidNetwork: boolean = false;
  public MMNetwork = environment.MMNetwork;
  public isBalanceBetter: boolean = false;
  public ethLimits = {
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
  private web3: Web3 = new Web3();
  private destroy$: Subject<boolean> = new Subject<boolean>();
  private gasLimit: number = 600000;
  private ethBalanceForCheck: BigNumber;
  private timeOut: any;

  constructor(
    private ethService: EthereumService,
    private cdRef: ChangeDetectorRef,
    private messageBox: MessageBoxService,
    private translate: TranslateService,
    private userService: UserService,
    private commonService: CommonService
  ) { }

  ngOnInit() {
    this.ethInput.valueChanges
      .debounceTime(500)
      .distinctUntilChanged()
      .takeUntil(this.destroy$)
      .subscribe(value => {
        if (this.fromEth && +value && !this.errors.invalidBalance && !this.errors.ethLimit) {
          this.estimateBuyOrder(this.eth, true, false);
        }
      });

    this.mntpInput.valueChanges
      .debounceTime(500)
      .distinctUntilChanged()
      .takeUntil(this.destroy$)
      .subscribe(value => {
        if (!this.fromEth && +value && !this.errors.invalidBalance && !this.errors.tokenLimit) {
          this.estimateBuyOrder(this.mntp, false, false);
        }
      });

    this.initTransactionHashModal();

    this.ethService.passEthBalance.takeUntil(this.destroy$).subscribe(eth => {
      this.ethBalanceForCheck = eth;

      if (eth) {
        this.ethService._contractInfura.getMaxGasPrice((err, res) => {
          let gas = (+res * this.gasLimit) / Math.pow(10, 18);
          this.ethBalance = +this.substrValue(+eth - gas);
          this.eth = +this.substrValue(+eth - gas);

          Observable.combineLatest(
            this.ethService.getObservableTokenDealRange(),
            this.ethService.getObservableEthDealRange()
          ).takeUntil(this.destroy$).subscribe(limits => {
            if (limits[0] && limits[1]) {
              this.tokenLimits.min = limits[0].min;
              this.tokenLimits.max = limits[0].max;

              this.ethLimits.min = limits[1].min;
              this.ethLimits.max = limits[1].max;
              this.estimateBuyOrder(this.eth, true, true);
            }
          });
        });
        this.cdRef.markForCheck();
      }
    });

    this.ethService.passEthAddress.takeUntil(this.destroy$).subscribe(address => {
      if (address) {
        if (!this.ethAddress) {
           this.timeOut = setTimeout(() => {
             !this.ethBalanceForCheck && this.translate.get('MESSAGE.MMHandingOut').subscribe(phrase => {
               this.messageBox.alert(`<div>${phrase}<div class="mm-hanging-out"></div></div>`).subscribe(ok => {
                 ok && location.reload();
               });
             });
          }, 3000);
        }

        this.ethAddress = address;
      }

      if (this.ethAddress && !address) {
        this.ethAddress = address;
        this.ethBalance = this.eth = this.mntp = 0;
      }
      this.cdRef.markForCheck();
    });

    this.ethService.getObservable1TokenPrice().takeUntil(this.destroy$).subscribe(price => {
      price && (this.buyPrice = price.buy);
      this.cdRef.markForCheck();
    });

    this.ethService.getObservableNetwork().takeUntil(this.destroy$).subscribe(network => {
      if (network !== null) {
        if (network != this.MMNetwork.index) {
          let networkName = this.MMNetwork.name;
          this.translate.get('MESSAGE.InvalidNetwork', {networkName}).subscribe(phrase => {
            setTimeout(() => {
              this.messageBox.alert(phrase);
            }, 0);
          });
          this.isInvalidNetwork = true;
        } else {
          this.isInvalidNetwork = false;
        }
        this.cdRef.markForCheck();
      }
    });

    this.commonService.isMobile$.takeUntil(this.destroy$).subscribe(isMobile => this.isMobile = isMobile);
  }

  changeValue(event, fromEth: boolean) {
    this.isTyping = true;
    this.fromEth = fromEth;

    event.target.value = this.substrValue(event.target.value);
    fromEth ? this.eth = +event.target.value : this.mntp = +event.target.value;

    this.checkErrors(fromEth, +event.target.value);
  }

  changeMinReturn(event) {
    event.target.value = this.substrValue(event.target.value);
    this.minReturn = +event.target.value;

    this.isMinReturnError = this.minReturn > this.mntp * this.minReturnPercent || this.minReturn <= 0;
    this.cdRef.markForCheck();
  }

  setCoinBalance(percent) {
    let value = this.substrValue(+this.ethBalance * percent);
    this.eth = +value;
    this.checkErrors(true, value);

    !this.errors.ethLimit && this.estimateBuyOrder(this.eth, true, false);
    this.cdRef.markForCheck();
  }

  substrValue(value) {
    return value.toString()
      .replace(',', '.')
      .replace(/([^\d.])|(^\.)/g, '')
      .replace(/^(\d{1,6})\d*(?:(\.\d{0,6})[\d.]*)?/, '$1$2')
      .replace(/^0+(\d)/, '$1');
  }

  estimateBuyOrder(amount: number, fromEth: boolean, isFirstLoad: boolean) {
    this.loading = true;
    this.isTyping = false;
    this.fromEth = fromEth;
    const wei = this.web3['toWei'](amount);

    this.ethService._contractInfura.estimateBuyOrder(wei, fromEth, (err, res) => {
      let estimate = +new BigNumber(res[0].toString()).div(new BigNumber(10).pow(18));
      this.estimateFee = +new BigNumber(res[1].toString()).div(new BigNumber(10).pow(18));
      this.averageTokenPrice = +new BigNumber(res[2].toString()).div(new BigNumber(10).pow(18));

      if (fromEth) {
        this.mntp = +this.substrValue(estimate);
      } else {
        this.eth = +this.substrValue(estimate);
        if (this.ethAddress && this.eth > this.ethBalance) {
          this.errors.invalidBalance = true;
        }
      }

      this.minReturn = +this.substrValue(this.mntp * this.minReturnPercent);

      this.loading = this.isMinReturnError = false;
      this.cdRef.markForCheck();
    });
    this.cdRef.markForCheck();
  }

  checkErrors(fromEth: boolean, value: number) {
    this.errors.invalidBalance = fromEth && this.ethAddress && this.eth > this.ethBalance;

    this.errors.ethLimit = fromEth && this.ethAddress && value > 0 &&
      (value < this.ethLimits.min || value > this.ethLimits.max);

    this.errors.tokenLimit = !fromEth && this.ethAddress && value > 0 &&
      (value < this.tokenLimits.min || value > this.tokenLimits.max);

    this.cdRef.markForCheck();
  }

  setMinReturn(percent: number) {
    !this.loading && (this.minReturn = +this.substrValue(this.mntp * percent));
    this.cdRef.markForCheck();
  }

  initTransactionHashModal() {
    this.ethService.getSuccessBuyRequestLink$.takeUntil(this.destroy$).subscribe(hash => {
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
      this.detectMetaMask('HeadingBuy');
      return;
    }

    let queryParams = {};
    window.location.hash.replace(/^[^?]*\?/, '').split('&').forEach(item => {
      let param = item.split('=');
      queryParams[decodeURIComponent(param[0])] = param.length > 1 ? decodeURIComponent(param[1]) : '';
    });
    let refAddress = queryParams['ref'] ? queryParams['ref'] : '0x0';

    this.ethService._contractInfura.getMaxGasPrice((err, res) => {
      if (+res) {
        const amount = this.web3['toWei'](this.eth);
        const minReturn = this.web3['toWei'](this.minReturn);
        this.ethService.buy(refAddress, this.ethAddress, amount, minReturn, +res);
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    clearTimeout(this.timeOut);
  }

}

import { Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import {UserService} from "../../services/user.service";
import {NavigationEnd, Router} from "@angular/router";
import {MainContractService} from "../../services/main-contract.service";
import {BigNumber} from "bignumber.js";
import {CommonService} from "../../services/common.service";
import {AllTokensBalance} from "../../interfaces/all-tokens-balance";
import {TranslateService} from "@ngx-translate/core";
import {MessageBoxService} from "../../services/message-box.service";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-header',
  templateUrl: './header-block.component.html',
  styleUrls: ['./header-block.component.sass'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderBlockComponent implements OnInit {

  public locale: string;
  public isShowMobileMenu: boolean = false;
  public isMobile: boolean = false;
  public userTotalReward: BigNumber | any = 0;
  public myBonusInfo: any = {
    shareReward: null,
    refBonus: null,
    promoBonus: null
  };
  public ethAddress: string = null;
  public isRefAvailable: boolean = null;
  public refLink: string;
  public isTradePage: boolean = false;
  public userRefLink: string;
  public myGenerateRefLink: string;
  public minRefTokenAmount: number = null;
  public allTokensBalance: AllTokensBalance[] = null;
  public allTokensBalanceSumEth: number = 0;
  public totalSpent: number = 0;
  public etherscanUrl = environment.etherscanUrl;

  private bonusPopTimer: any;

  constructor(
    private cdRef: ChangeDetectorRef,
    private userService: UserService,
    private mainContractService: MainContractService,
    private router: Router,
    private commonService: CommonService,
    private messageBox: MessageBoxService,
    private translate: TranslateService
  ) {
    router.events.subscribe(route => {
      if (route instanceof NavigationEnd) {
        let queryParams = {};
        window.location.hash.replace(/^[^?]*\?/, '').split('&').forEach(item => {
          let param = item.split('=');
          queryParams[decodeURIComponent(param[0])] = param.length > 1 ? decodeURIComponent(param[1]) : '';
        });

        !this.userRefLink && queryParams['ref'] && (this.userRefLink = queryParams['ref']);
        this.userRefLink && !queryParams['ref'] && (window.location.href = window.location.href + '?ref=' + this.userRefLink);

        this.isShowMobileMenu = false;
        document.body.style.overflow = 'visible';
        this.cdRef.markForCheck();
      }
    })
  }

  ngOnInit() {
    this.commonService.initMainContract$.next(true);
    this.userService.currentLocale.subscribe(currentLocale => {
      this.locale = currentLocale;
      this.cdRef.markForCheck();
    });

    this.initTransactionHashModal();

    window.innerWidth > 992 ? this.isMobile = this.isShowMobileMenu = false : this.isMobile = true;
    this.commonService.isMobile$.next(this.isMobile);

    window.onresize = () => {
      if (window.innerWidth > 992) {
        this.isMobile = this.isShowMobileMenu = false;
        document.body.style.overflow = 'visible';
      } else {
        this.isMobile = true;
      }
      this.commonService.isMobile$.next(this.isMobile);
      this.cdRef.markForCheck();
    };

    this.mainContractService.getObservableEthAddress().subscribe(address => {
      if (this.ethAddress && !address) {
        this.isRefAvailable = false;
        this.userTotalReward = 0;
      }
      this.ethAddress = address;
      this.myGenerateRefLink = `${window.location.origin}/#/market?ref=${this.ethAddress}`;
      this.cdRef.markForCheck();
    });

    this.mainContractService.getObservableUserTotalReward().subscribe(reward => {
      if (reward) {
        if (this.isRefAvailable === null || +this.userTotalReward !== +reward) {
          this.checkRefAvailable();

          this.mainContractService._contractMetamask.getCurrentUserShareBonus((err, res) => {
            this.myBonusInfo.shareReward = +new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
          });
          this.mainContractService._contractMetamask.getCurrentUserRefBonus((err, res) => {
            this.myBonusInfo.refBonus = +new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
          });
          this.mainContractService._contractMetamask.getCurrentUserPromoBonus((err, res) => {
            this.myBonusInfo.promoBonus = +new BigNumber(res.toString()).div(new BigNumber(10).pow(18));
          });
        }
        this.userTotalReward = reward;
      }
      this.cdRef.markForCheck();
    });

    this.mainContractService.passTokensBalance$.subscribe((balances: AllTokensBalance[]) => {
      if (balances) {
        this.allTokensBalanceSumEth = 0;
        this.allTokensBalance = balances;
        balances.forEach(item => {
          this.allTokensBalanceSumEth += item.estimate;
        });
        this.cdRef.markForCheck();
      }
    });
    this.cdRef.markForCheck();
  }

  checkRefAvailable() {
    this.mainContractService._contractMetamask._minRefEthPurchase((err, res) => {
      this.minRefTokenAmount = +res / Math.pow(10, 18);

      this.mainContractService._contractMetamask.getUserTotalEthVolumeSaldo(this.ethAddress, (err, res) => {
        this.totalSpent = +res / Math.pow(10, 18);
        this.cdRef.markForCheck();
      });
      this.cdRef.markForCheck();
    });

    this.mainContractService._contractMetamask.isRefAvailable((err, res) => {
      this.isRefAvailable = res;
      this.mainContractService.isRefAvailable$.next({isAvailable: res});
      this.cdRef.markForCheck();
    });
  }

  toggleMobileMenu(e) {
    this.isShowMobileMenu = !this.isShowMobileMenu;
    document.body.style.overflow = this.isShowMobileMenu ? 'hidden' : 'visible';
    e.stopPropagation();
    this.cdRef.markForCheck();
  }

  withdraw() {
    this.mainContractService.withdraw();
  }

  onCopyData(input) {
    input.focus();
    input.setSelectionRange(0, input.value.length);
    document.execCommand("copy");
    input.setSelectionRange(0, 0);
  }

  popEnter(pop) {
    setTimeout(() => {
      let popoverContainer = document.querySelector('.popover-content');
      popoverContainer && popoverContainer.addEventListener('mouseenter', () => clearTimeout(this.bonusPopTimer));
      popoverContainer && popoverContainer.addEventListener('mouseleave', () => pop.hide());
    }, 0);
  }

  popLeave(pop) {
    this.bonusPopTimer = setTimeout(() => pop.hide(), 300);
  }

  initTransactionHashModal() {
    this.mainContractService.getSuccessWithdrawRequestLink$.subscribe(hash => {
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

}

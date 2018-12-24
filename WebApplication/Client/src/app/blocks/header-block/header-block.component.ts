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

let self;

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
  public trxAddress: string = null;
  public isRefAvailable: boolean = null;
  public refLink: string;
  public isTradePage: boolean = false;
  public userRefLink: string;
  public myGenerateRefLink: string;
  public minRefTokenAmount: number = null;
  public allTokensBalance: AllTokensBalance[] = null;
  public allTokensBalanceSumTrx: number = 0;
  public totalSpent: number = 0;
  public tronscanUrl = environment.tronscanUrl;

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
    self = this;
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

    this.mainContractService.getObservableTronAddress().subscribe(address => {
      if (this.trxAddress && !address) {
        this.isRefAvailable = false;
        this.userTotalReward = 0;
      }
      this.trxAddress = address;
      this.myGenerateRefLink = `${window.location.origin}/#/market?ref=${this.trxAddress}`;
      this.cdRef.markForCheck();
    });

    this.mainContractService.getObservableUserTotalReward().subscribe(reward => {
      if (reward !== null && this.trxAddress) {
        if (this.isRefAvailable === null || +this.userTotalReward !== +reward) {
          this.checkRefAvailable();

          (async function init() {
            let res = +await self.mainContractService.fotronCoreContract.getCurrentUserShareBonus().call();
            self.myBonusInfo.shareReward = res / Math.pow(10, 6);

            let res2 = +await self.mainContractService.fotronCoreContract.getCurrentUserRefBonus().call();
            self.myBonusInfo.refBonus = res2 / Math.pow(10, 6);

            let res3 = +await self.mainContractService.fotronCoreContract.getCurrentUserPromoBonus().call();
            self.myBonusInfo.promoBonus = res3 / Math.pow(10, 6);
          })();
        }
        this.userTotalReward = reward;
      }
      this.cdRef.markForCheck();
    });

    this.mainContractService.passTokensBalance$.subscribe((balances: AllTokensBalance[]) => {
      if (balances) {
        this.allTokensBalanceSumTrx = 0;
        this.allTokensBalance = balances;
        balances.forEach(item => {
          this.allTokensBalanceSumTrx += item.estimate;
        });
        this.cdRef.markForCheck();
      }
    });
    this.cdRef.markForCheck();
  }

  checkRefAvailable() {
    (async function init() {
      let res = +await self.mainContractService.fotronCoreContract._minRefTrxPurchase().call();
      self.minRefTokenAmount = res / Math.pow(10, 6);

      let res2 = await self.mainContractService.fotronCoreContract.getUserTotalTrxVolumeSaldo(self.trxAddress).call();
      self.totalSpent = +res2.res / Math.pow(10, 6);

      let res3 = await self.mainContractService.fotronCoreContract.isRefAvailable().call();
      self.isRefAvailable = res3;
      self.mainContractService.isRefAvailable$.next({isAvailable: res3});
      self.cdRef.markForCheck();
    })();
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
              <a href="${this.tronscanUrl}${hash}" target="_blank">${phrases.Link}</a>
            </div>
          `);
        });
      }
    });
  }

}

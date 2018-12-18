import {ChangeDetectorRef, Component, OnDestroy, OnInit} from '@angular/core';
import {Subject} from "rxjs/Subject";
import {CommonService} from "../../../services/common.service";
import {TronService} from "../../../services/tron.service";

@Component({
  selector: 'app-promo-bonus',
  templateUrl: './promo-bonus.component.html',
  styleUrls: ['./promo-bonus.component.sass']
})
export class PromoBonusComponent implements OnInit, OnDestroy {

  public promoBonus = {
    big: 0,
    quick: 0
  }
  public bigWinPromoBonus: number = 0;
  public quickWinPromoBonus: number = 0;
  public bigBankTimer;
  public quickBankTimer;
  public isDataLoaded: boolean = false;
  public isFirstLoad: boolean = true;

  private destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private tronService: TronService,
    private commonService: CommonService,
    private cdRef: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.tronService.getObservablePromoBonus().takeUntil(this.destroy$).subscribe(bonus => {
      if (bonus) {
        this.promoBonus.big = +bonus.big;
        this.promoBonus.quick = +bonus.quick;

        this.promoBonus.big = this.promoBonus.big < Math.pow(10, -9) ? Math.pow(10, -9) : +this.promoBonus.big.toFixed(9);
        this.promoBonus.quick = this.promoBonus.quick < Math.pow(10, -9) ? Math.pow(10, -9) : +this.promoBonus.quick.toFixed(9);

        !this.isFirstLoad && this.commonService.isDataLoaded$.next(true);
        this.cdRef.markForCheck();
      }
      this.isFirstLoad = false;
    });

    this.tronService.getObservableWinBIGPromoBonus().takeUntil(this.destroy$).subscribe(bonus => {
      if (bonus) {
        this.bigWinPromoBonus = bonus;
        this.bigBankTimer = new Date().getTime() + (this.bigWinPromoBonus * 15000);
        this.cdRef.markForCheck();
      }
    });

    this.tronService.getObservableWinQUICKPromoBonus().takeUntil(this.destroy$).subscribe(bonus => {
      if (bonus) {
        this.quickWinPromoBonus = bonus;
        this.quickBankTimer = new Date().getTime() + (this.quickWinPromoBonus * 15000);
        this.cdRef.markForCheck();
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next(true);
  }

}

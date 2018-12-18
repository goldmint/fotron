import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {Subject} from "rxjs/Subject";
import {MainContractService} from "../../../services/main-contract.service";

@Component({
  selector: 'app-main-promo-bonus',
  templateUrl: './main-promo-bonus.component.html',
  styleUrls: ['./main-promo-bonus.component.sass']
})
export class MainPromoBonusComponent implements OnInit {

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
    private mainContractService: MainContractService,
    private cdRef: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.mainContractService.getObservablePromoBonus().takeUntil(this.destroy$).subscribe(bonus => {
      if (bonus) {
        this.promoBonus.big = +bonus.big;
        this.promoBonus.quick = +bonus.quick;

        this.promoBonus.big = this.promoBonus.big < Math.pow(10, -9) ? Math.pow(10, -9) : +this.promoBonus.big.toFixed(9);
        this.promoBonus.quick = this.promoBonus.quick < Math.pow(10, -9) ? Math.pow(10, -9) : +this.promoBonus.quick.toFixed(9);

        this.isDataLoaded = true;
        this.cdRef.markForCheck();
      }
      this.isFirstLoad = false;
    });

    this.mainContractService.getObservableWinBIGPromoBonus().takeUntil(this.destroy$).subscribe(bonus => {
      if (bonus) {
        this.bigWinPromoBonus = bonus;
        this.bigBankTimer = new Date().getTime() + (this.bigWinPromoBonus * 15000);
        this.cdRef.markForCheck();
      }
    });

    this.mainContractService.getObservableWinQUICKPromoBonus().takeUntil(this.destroy$).subscribe(bonus => {
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

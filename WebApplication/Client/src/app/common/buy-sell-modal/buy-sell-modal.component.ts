import {ChangeDetectorRef, Component, OnDestroy, OnInit, ViewEncapsulation} from '@angular/core';
import {BsModalRef} from "ngx-bootstrap/modal";
import {CommonService} from "../../services/common.service";
import {MarketData} from "../../interfaces/market-data";
import {Subject} from "rxjs/Subject";
import {APIService} from "../../services/api.service";
import {TokenInfo} from "../../interfaces/token-info";

@Component({
  selector: 'app-buy-sell-modal',
  templateUrl: './buy-sell-modal.component.html',
  styleUrls: ['./buy-sell-modal.component.sass'],
  encapsulation: ViewEncapsulation.None
})
export class BuySellModalComponent implements OnInit, OnDestroy {

  public isStop: boolean = true;
  public switchModel: {
    type: 'buy'|'sell'
  };
  public isDataLoaded: boolean = false;
  public tokenInfo: TokenInfo;

  private destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private bsModalRef: BsModalRef,
    private commonService: CommonService,
    private apiService: APIService,
    private cdRef: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.commonService.passMarketData$.takeUntil(this.destroy$).subscribe((data: MarketData) => {
      if (data) {
        this.apiService.getTokenInfo(data.tokenId).subscribe((data: any) => {
          this.tokenInfo = data.data;
          this.isDataLoaded = true;
          this.cdRef.markForCheck();
        });
      }
    });

    this.switchModel = {
      type: 'buy'
    };

    setTimeout(() => {
      this.isStop = this.bsModalRef.content.isStop;
    }, 0);
  }

  public hide() {
    this.bsModalRef.hide();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.isStop && this.commonService.passMarketData$.next(null);
  }
}

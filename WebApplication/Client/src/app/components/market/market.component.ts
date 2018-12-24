import {
  ChangeDetectorRef,
  Component,
  HostBinding,
  HostListener, OnDestroy,
  OnInit,
  TemplateRef,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import {Subject} from "rxjs/Subject";
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {UserService} from "../../services/user.service";
import {APIService} from "../../services/api.service";
import {TranslateService} from "@ngx-translate/core";
import {Page} from "../../models/page.model";
import {TokenList} from "../../interfaces/token-list";
import {Router} from "@angular/router";
import {CommonService} from "../../services/common.service";
import {MessageBoxService} from "../../services/message-box.service";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-market',
  templateUrl: './market.component.html',
  styleUrls: ['./market.component.sass'],
  encapsulation: ViewEncapsulation.None
})
export class MarketComponent implements OnInit, OnDestroy {

  @ViewChild('searchInput') searchInput;
  @HostBinding('class') class = 'page';
  @HostListener('window:resize', ['$event'])
  onResize(event) {
    let isMobile = event.target.innerWidth <= 992;
    this.isMobile !== isMobile && this.setPage();
    this.isMobile = isMobile;
  }

  public page = new Page();
  public originalRows: TokenList[] = [];
  public searchRows: TokenList[] = [];
  public rows: TokenList[] = [];
  public messages: any  = {emptyMessage: 'No data'};
  public isMobile: boolean = false;
  public loading: boolean = false;
  public isDataLoaded: boolean = false;
  public searchValue: string = '';
  public coreContractTronscanLink = environment.tronscanContractUrl + environment.fotronCoreContractAddress

  private charts: any = {};
  private miniCharts: any = [];
  private destroy$: Subject<boolean> = new Subject<boolean>();

  modalRef: BsModalRef;

  constructor(
    private modalService: BsModalService,
    private userService: UserService,
    private commonService: CommonService,
    private apiService: APIService,
    private translate: TranslateService,
    private router: Router,
    private messageBox: MessageBoxService,
    private cdRef: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.isMobile = (window.innerWidth <= 992);
    this.page.pageNumber = 0;
    this.page.size = 10;

    this.searchInput.valueChanges
      .debounceTime(500)
      .distinctUntilChanged()
      .takeUntil(this.destroy$)
      .subscribe(value => {
        this.searchInput.dirty && this.search();
      });

    this.init();

    this.userService.currentLocale.takeUntil(this.destroy$).subscribe(() => {
      if (this.isDataLoaded) {
        this.translate.get('PAGES.Market.Chart.Detail').subscribe(phrase => {
          this.charts.chart && this.charts.chart.title(phrase);
        });
      }
    });
  }

  init() {
    this.loading = true;
    this.apiService.getTokenList()
      .finally(() => {
        this.loading = false;
        this.cdRef.markForCheck();
      })
      .subscribe((data: any) => {
        this.originalRows = data.data || [];

        this.originalRows.forEach(row => {
          row.chartData = [];
          let currentDate = new Date();
          for (let i = row.priceStatistics7D.length - 1; i >= 0; i--) {
            let month = (currentDate.getMonth()+1).toString();
            let day = (currentDate.getDate()).toString();

            let dateString = currentDate.getFullYear() + '-'
                             + (month.length > 1 ? month : '0' + month) + '-'
                             + (day.length >1 ? day : '0' + day);

            row.chartData.unshift([dateString, row.priceStatistics7D[i]]);
            currentDate.setDate(currentDate.getDate()-1);
          };
        });

        this.calculateTotalPage(this.originalRows);

        this.setPage();
        this.isDataLoaded = true;
        this.cdRef.markForCheck();
      });
  }

  setPage() {
    let currentRows = this.searchValue ? this.searchRows : this.originalRows;
    this.rows = this.getCurrentPage(currentRows);

    this.rows.forEach((row, index) => {
      setTimeout(() => {
        this.createMiniChart(row.chartData, index);
      }, 0);
    });
    this.cdRef.markForCheck();
  }

  changePage(pageNumber: number) {
    this.setPage();
  }

  createMiniChart(data: any[], i: number) {
    anychart.onDocumentReady( () => {
      this.miniCharts[i] = {};
      const container = 'chart-container-' + i;
      const child = document.querySelector(`#${container} > div`);
      child && child.remove();

      this.miniCharts[i].table = anychart.data.table();
      this.miniCharts[i].table.addData(data);

      this.miniCharts[i].mapping = this.miniCharts[i].table.mapAs();
      this.miniCharts[i].mapping.addField('value', 1);

      this.miniCharts[i].chart = anychart.stock();
      this.miniCharts[i].chart.scroller().enabled(false);
      this.miniCharts[i].chart.crosshair(false);
      this.miniCharts[i].chart.plot(0).line(this.miniCharts[i].mapping).name('TRX');

      this.miniCharts[i].chart.plot(0).xAxis().enabled(false);
      this.miniCharts[i].chart.plot(0).yAxis().enabled(false);
      this.miniCharts[i].chart.plot(0).legend().enabled(false);

      if (document.getElementById(container)) {
        this.miniCharts[i].chart.container(container);
        this.miniCharts[i].chart.draw();
      }
    });
  }

  showDetailsChart(data: any[], template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
    document.querySelector('modal-container').classList.add('modal-chart');
    this.initDetailsChart(data);
  }

  initDetailsChart(data: any[]) {
    anychart.onDocumentReady( () => {
      this.charts.table && this.charts.table.remove();

      this.charts.table = anychart.data.table();
      this.charts.table.addData(data);

      this.charts.mapping = this.charts.table.mapAs();
      this.charts.mapping.addField('value', 1);

      this.charts.chart = anychart.stock();
      this.charts.chart.scroller().enabled(false);

      this.charts.chart.plot(0).line(this.charts.mapping).name('TRX');
      this.charts.chart.plot(0).legend().itemsFormatter(() => {
        return [
          {text: "Token price", iconFill:"#63B7F7"}
        ]
      });

      this.translate.get('PAGES.Market.Chart.Detail').subscribe(phrase => {
        this.charts.chart.title(phrase);
      });

      this.charts.chart.container('details-chart-container');
      this.charts.chart.draw();
    });
  }

  search() {
    this.page.pageNumber = 0;
    this.cdRef.markForCheck();

    if (this.searchValue === '') {
      this.setPage();
      this.calculateTotalPage(this.originalRows);
      return;
    }

    this.searchRows = this.originalRows.filter(row => row.ticker.toLowerCase().indexOf(this.searchValue.toLowerCase()) >= 0);
    this.setPage();
    this.calculateTotalPage(this.searchRows);
  }

  getCurrentPage(rows) {
    return rows.slice(this.page.pageNumber * this.page.size, (this.page.pageNumber + 1) * this.page.size);
  }

  calculateTotalPage(rows) {
    this.page.totalElements = rows.length;
    this.page.totalPages = Math.ceil(this.page.totalElements / this.page.size);
  }

  moreInfo(row: TokenList) {
    this.router.navigate(['/trade', row.fotronContractAddress]);
  }

  trade(row: TokenList) {
    let data: any = {};
    data.fotronContractAddress = row.fotronContractAddress;
    data.tokenContractAddress = row.tokenContractAddress;
    data.tokenId = row.id;
    data.isTradePage = false;
    this.commonService.passMarketData$.next(data);
    this.messageBox.buySell(true);
  }

  ngOnDestroy() {
    this.destroy$.next(true);
  }

}

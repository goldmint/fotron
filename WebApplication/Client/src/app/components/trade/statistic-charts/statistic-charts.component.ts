import {Component, OnDestroy, OnInit} from '@angular/core';
import 'anychart';
import {TranslateService} from "@ngx-translate/core";
import {Subject} from "rxjs/Subject";
import {UserService} from "../../../services/user.service";
import {MarketData} from "../../../interfaces/market-data";
import {APIService} from "../../../services/api.service";
import {TokenStatistics} from "../../../interfaces/token-statistics";
import {CommonService} from "../../../services/common.service";

@Component({
  selector: 'app-statistic-charts',
  templateUrl: './statistic-charts.component.html',
  styleUrls: ['./statistic-charts.component.sass']
})
export class StatisticChartsComponent implements OnInit, OnDestroy {

  public charts = {
    priceEth: {
      chart: {},
      data: [],
      id: 'priceEth',
      fieldName: ['priceEth'],
      options: [
        {text: "Token price", iconFill:"#63B7F7", label: 'ETH'}
      ]
    },
    shareReward: {
      chart: {},
      data: [],
      id: 'shareReward',
      fieldName: ['shareReward'],
      options: [
        {text: "Share bonus", iconFill:"#63B7F7", label: 'ETH'}
      ]
    },
    buySellCount: {
      chart: {},
      data: [],
      id: 'buySellCount',
      fieldName: ['totalTxCount'],
      options: [
        {text: "Tx amount", iconFill:"#63B7F7", label: 'TX'},
      ]
    }
  };
  public tokenId: number;
  public tokenStatistics: TokenStatistics[];

  private destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private translate: TranslateService,
    private apiService: APIService,
    private userService: UserService,
    private commonService: CommonService
  ) { }

  ngOnInit() {
    this.commonService.passMarketData$.takeUntil(this.destroy$).subscribe((data: MarketData) => {
      if (data) {
        this.tokenId = data.tokenId;
        this.apiService.getTokenStatistic(0, data.tokenId, 0).subscribe((data: any) => {
          this.tokenStatistics = data.data;

          for (let chart in this.charts) {
            let currentChart = this.charts[chart];
            this.setChartsData(this.tokenStatistics, currentChart.fieldName, currentChart.data);
            this.initDailyStatChart(currentChart.chart, currentChart.data, currentChart.id, currentChart.options);
          }
        });
      }
    });
  }

  setChartsData(data: TokenStatistics[], fieldName: string[], chartDataSource: any[]) {
    if (data) {
      data.forEach(item => {
        let date = new Date(item.date.toString() + 'Z'),
            month = (date.getMonth()+1).toString(),
            day = date.getDate().toString();

        let dateString = date.getFullYear() + '-'
                         + (month.length > 1 ? month : '0' + month) + '-'
                         + (day.length > 1 ? day : '0' + day);

        let arr = [dateString];
        fieldName.forEach(field => {
          arr.push(item[field]);
        });
        chartDataSource.push(arr);
      });
      }
  }

  initDailyStatChart(chart: any, data: any[], id: string, options: any[]) {
    anychart.onDocumentReady( () => {
      chart.table = anychart.data.table();
      chart.table.addData(data);

      chart.chart = anychart.stock();
      chart.chart.scroller().enabled(false);

      options.forEach((item, i) => {
        chart.chart.plot(0).line(chart.table.mapAs({value: i+1})).name(item.label).stroke({
          color: item.iconFill,
          thickness: 2
        });
      });

      chart.chart.plot(0).legend().itemsFormatter(() => {
        return options;
      });

      this.userService.currentLocale.takeUntil(this.destroy$).subscribe(() => {
        this.translate.get('PAGES.Statistics.Charts.Headings.' + id).subscribe(phrase => {
          chart.chart.title(phrase);
        });
      });

      let containerId = 'chart-container-' + id;
      chart.chart.container(containerId);
      chart.chart.draw();
    });
  }

  ngOnDestroy() {
    this.destroy$.next(true);
  }

}

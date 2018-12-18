import { Injectable } from '@angular/core';
import {MarketData} from "../interfaces/market-data";
import {BehaviorSubject} from "rxjs/BehaviorSubject";
import {Subject} from "rxjs/Subject";

@Injectable()
export class CommonService {

  public passMarketData$ = new BehaviorSubject<MarketData>(null);
  public initMainContract$ = new BehaviorSubject<boolean>(null);
  public isMobile$ = new BehaviorSubject<boolean>(null);
  public isDataLoaded$ = new Subject();

  constructor() { }

}

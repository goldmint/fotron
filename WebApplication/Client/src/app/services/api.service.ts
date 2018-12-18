import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {AddTokenRequest} from "../models/add-token-request";

@Injectable()
export class APIService {

  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getTokenList() {
    return this.http.get(`${this.baseUrl}/token/list`);
  }

  getTokenStatistic(dateFrom: number, tokenId: number, dateTo: number) {
    return this.http.get(`${this.baseUrl}/token/stat?DateFrom=${dateFrom}&Id=${tokenId}&DateTo=${dateTo}`);
  }

  getTokenInfo(tokenId: number) {
    return this.http.get(`${this.baseUrl}/token/full-info?Id=${tokenId}`);
  }

  addTokenRequest(params: AddTokenRequest) {
    return this.http.post(`${this.baseUrl}/token/add-request`, params);
  }
}

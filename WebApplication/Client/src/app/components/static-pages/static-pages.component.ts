import {Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";

enum Pages {serviceAgreement, termsOfService}

@Component({
  selector: 'app-static-pages',
  templateUrl: './static-pages.component.html',
  styleUrls: ['./static-pages.component.sass']
})
export class StaticPagesComponent implements OnInit {
  public pagePath:string;
  private _pages = Pages;
  private _links = [
    'assets/docs/service_agreement.pdf',
    'assets/docs/terms_of_service.pdf'
  ];

  constructor(
      private _route: ActivatedRoute,
  ) {
    this._route.params
        .subscribe(params => {
          let page = params.page;
          if (page) {
            this.pagePath = this._links[this._pages[page]];
          }
        })
  }

  ngOnInit() {
  }

}

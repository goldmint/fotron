import { NgModule } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import {HttpClient, HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { AppRouting } from './app.routing';

import { APIHttpInterceptor } from './common/api/api-http.interceptor';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
registerLocaleData(localeRu);
import { RECAPTCHA_SETTINGS,
  RecaptchaModule
} from 'ng-recaptcha';

import {
  BsDropdownModule,
  ModalModule,
  ButtonsModule, CollapseModule, PopoverModule, CarouselModule
} from 'ngx-bootstrap';

import { HeaderBlockComponent } from './blocks/header-block/header-block.component';
import { NavbarBlockComponent } from './blocks/navbar-block/navbar-block.component';
import { MessageBoxComponent } from './common/message-box/message-box.component';
import { SpriteComponent } from './common/sprite/sprite.component';
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {MessageBoxService} from "./services/message-box.service";
import {APIService} from "./services/api.service";
import {NotFoundPageComponent} from "./components/not-found-page/not-found-page.component";
import {LanguageSwitcherBlockComponent} from "./blocks/language-switcher-block/language-switcher-block.component";
import {UserService} from "./services/user.service";
import {BuyComponent} from "./components/trade/buy/buy.component";
import {SellComponent} from "./components/trade/sell/sell.component";
import {SubstrPipe} from "./pipes/substr.pipe";
import {NoexpPipe} from "./pipes/noexp.pipe";
import { PromoBonusComponent } from './components/trade/promo-bonus/promo-bonus.component';
import { AddTokenComponent } from './components/add-token/add-token.component';
import { TimerComponent } from './common/timer/timer.component';
import { StatisticChartsComponent } from './components/trade/statistic-charts/statistic-charts.component';
import { BuySellModalComponent } from './common/buy-sell-modal/buy-sell-modal.component';
import { FaqComponent } from './components/faq/faq.component';
import { MarketComponent } from './components/market/market.component';
import { TradeComponent } from './components/trade/trade.component';
import {CommonService} from "./services/common.service";
import { EthAddressValidatorDirective } from './directives/eth-address-validator.directive';
import {environment} from "../environments/environment";
import { EmailValidatorDirective } from './directives/email-validator.directive';
import { MainPromoBonusComponent } from './components/market/main-promo-bonus/main-promo-bonus.component';
import {MainContractService} from "./services/main-contract.service";
import {ShareButtonsModule} from "@ngx-share/buttons";
import {FooterBlockComponent} from "./blocks/footer-block/footer-block.component";
import {StaticPagesComponent} from "./components/static-pages/static-pages.component";
import {LegalSecurityPageComponent} from "./components/legal-security-page/legal-security-page.component";
import {SafePipe} from "./pipes/safe.pipe";
import {FotronMainModalComponent} from "./common/fotron-main-modal/fotron-main-modal.component";
import {TronService} from "./services/tron..service";

export function createTranslateLoader(http: HttpClient) {
    return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  imports: [
    AppRouting,
    HttpClientModule,
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    BsDropdownModule.forRoot(),
    ModalModule.forRoot(),
    ButtonsModule.forRoot(),
    CollapseModule.forRoot(),
    RecaptchaModule.forRoot(),
    HttpClientModule,
    NgxDatatableModule,
    PopoverModule.forRoot(),
    CarouselModule.forRoot(),
    ShareButtonsModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      }
    })
  ],
  declarations: [
    AppComponent,
    LanguageSwitcherBlockComponent,
    HeaderBlockComponent,
    NavbarBlockComponent,
    MessageBoxComponent,
    SpriteComponent,
    NotFoundPageComponent,
    BuyComponent,
    SellComponent,
    SubstrPipe,
    NoexpPipe,
    PromoBonusComponent,
    AddTokenComponent,
    TimerComponent,
    StatisticChartsComponent,
    BuySellModalComponent,
    FaqComponent,
    MarketComponent,
    TradeComponent,
    EthAddressValidatorDirective,
    EmailValidatorDirective,
    MainPromoBonusComponent,
    FotronMainModalComponent,
    FooterBlockComponent,
    StaticPagesComponent,
    LegalSecurityPageComponent,
    SafePipe
  ],
  exports: [],
  providers: [
    Title,
    MessageBoxService,
    APIService,
    UserService,
    CommonService,
    TronService,
    MainContractService,
    {
      provide: RECAPTCHA_SETTINGS,
      useValue: {
        siteKey: environment.recaptchaSiteKey
      }
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: APIHttpInterceptor,
      multi: true
    }
  ],
  entryComponents: [
    MessageBoxComponent,
    BuySellModalComponent,
    FotronMainModalComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

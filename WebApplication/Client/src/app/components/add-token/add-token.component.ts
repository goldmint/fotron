import {ChangeDetectionStrategy, ChangeDetectorRef, Component, HostBinding, OnInit, ViewChild} from '@angular/core';
import {AddToken} from "../../models/add-token";
import { RecaptchaComponent as reCaptcha } from 'ng-recaptcha';
import {AddTokenRequest} from "../../models/add-token-request";
import {MessageBoxService} from "../../services/message-box.service";
import {TranslateService} from "@ngx-translate/core";
import {APIService} from "../../services/api.service";
import 'rxjs/add/operator/finally';
import {Router} from "@angular/router";

@Component({
  selector: 'app-add-token',
  templateUrl: './add-token.component.html',
  styleUrls: ['./add-token.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AddTokenComponent implements OnInit {

  @HostBinding('class') class = 'page';
  @ViewChild('form') from;
  @ViewChild('captchaRef') captchaRef: reCaptcha;

  public addTokenData: AddToken = new AddToken();
  public controlsMap = {
    1: 'company',
    2: 'email',
    3: 'webSite',
    4: 'ticker',
    5: 'price',
    6: 'isCreate',
    7: 'contract',
    8: 'supply'
  }
  public currentStep: number = 1;
  public lastStep: number = Object.keys(this.controlsMap).length;
  public invalidField: boolean = true;
  public recaptcha: any = null;
  public loading: boolean = false;

  constructor(
    private messageBox: MessageBoxService,
    private translate: TranslateService,
    private apiService: APIService,
    private router: Router,
    private cdRef: ChangeDetectorRef
  ) { }

  ngOnInit() { }

  checkField(isNext: boolean) {
    this.invalidField = false;

    if (this.currentStep === 7 && this.addTokenData.isCreate) {
      this.currentStep = isNext ? this.currentStep+1 : this.currentStep-1;
    }

    let control = this.from.controls[this.controlsMap[this.currentStep]];
    this.invalidField = control && control.invalid;
    this.cdRef.markForCheck();
  }

  prevStep(isNext: boolean) {
    this.recaptcha && this.captchaRef && this.captchaRef.reset();
    this.currentStep--;
    this.checkField(isNext);
  }

  nextStep(isNext: boolean) {
    this.currentStep++;
    this.checkField(isNext);
  }

  public captchaResolved(captchaResponse: string) {
    this.recaptcha = captchaResponse;
    this.cdRef.markForCheck();
  }

  addToken() {
    if (this.currentStep === this.lastStep) {
      let params: AddTokenRequest = new AddTokenRequest();
      params.companyName = this.addTokenData.company;
      params.websiteUrl = this.addTokenData.webSite;
      params.contactEmail = this.addTokenData.email;
      params.tokenTicker = this.addTokenData.ticker;
      params.tokenContractAddress = !this.addTokenData.isCreate ? this.addTokenData.contract : '';
      params.startPriceEth = this.addTokenData.price;
      params.totalSupply = this.addTokenData.supply;

      this.loading = true;
      this.apiService.addTokenRequest(params)
        .finally(() => {
          this.loading = false;
          this.captchaRef && this.captchaRef.reset();
          this.cdRef.markForCheck();
        }).subscribe(() => {
          this.translate.get('MESSAGE.AddToken').subscribe(phrase => {
            this.messageBox.alert(phrase);
          });
          this.router.navigate(['/market']);
      });
      this.cdRef.markForCheck();
    }
  }
}

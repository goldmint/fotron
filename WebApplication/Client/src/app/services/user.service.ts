import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { AppDefaultLanguage } from '../app.languages';
import {MessageBoxService} from "./message-box.service";
import {TranslateService} from "@ngx-translate/core";

@Injectable()
export class UserService {

  private _locale = new BehaviorSubject<string>(AppDefaultLanguage || 'en');
  public currentLocale: Observable<string> = this._locale.asObservable();

  constructor(
    private messageBox: MessageBoxService,
    private translate: TranslateService
  ) { }

  public setLocale(locale: string) {
    this._locale.next(locale);
  }

  loginToMM(heading: string) {
    this.translate.get('MESSAGE.LoginToMM').subscribe(phrase => {
      this.messageBox.alert(`
        <div class="text-center">${phrase.Text}</div>
        <div class="metamask-icon"></div>
        <div class="text-center mt-2 mb-2">MetaMask</div>
      `, phrase[heading]);
    });
  }
}

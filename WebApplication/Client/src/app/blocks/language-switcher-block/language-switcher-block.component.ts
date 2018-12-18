import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';

import { AppLanguages, AppDefaultLanguage } from '../../app.languages';
import {UserService} from "../../services/user.service";

@Component({
  selector: 'app-language-switcher',
  templateUrl: './language-switcher-block.component.html',
  styleUrls: ['./language-switcher-block.component.sass'],
  encapsulation: ViewEncapsulation.None,
})
export class LanguageSwitcherBlockComponent implements OnInit {

  public defaultLanguage: string = AppDefaultLanguage || 'en';

  public languages = {
    en: {
      name: 'English',
      icon: 'gb.png',
      locale: 'en'
    }
  }

  public constructor(
    private userService: UserService,
    public translate: TranslateService) {

    Object.assign(this.languages, AppLanguages);

    const codes = Object.keys(this.languages);

    let userLanguage = localStorage.getItem('gmint_language')
      ? localStorage.getItem('gmint_language')
      : translate.getBrowserLang()
    ;

    userLanguage = userLanguage.match('^(' + codes.join('|') + ')$')
      ? userLanguage
      : this.defaultLanguage;

    translate.addLangs(codes);
    translate.setDefaultLang(this.defaultLanguage);
    translate.use(userLanguage);

    userService.setLocale(userLanguage);
  }

  ngOnInit() {
    this.translate.onLangChange.subscribe((event: LangChangeEvent) => {
      this.userService.setLocale(event.lang);
      localStorage.setItem('gmint_language', event.lang);
    });
  }

}

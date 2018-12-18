import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/catch';

import { MessageBoxService } from '../../services/message-box.service';
import {TranslateService} from "@ngx-translate/core";

@Injectable()
export class APIHttpInterceptor implements HttpInterceptor {

  constructor(
    private _messageBox: MessageBoxService,
    private translate: TranslateService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    return next.handle(req)
      .catch((error, caught) => {
        this.translate.get('MESSAGE.ServiceUnavailable').subscribe(phrase => {
          this._messageBox.alert(phrase);
        });

        return Observable.throw(error);
      }) as any;
  }

}

import { Directive } from '@angular/core';
import {FormControl, NG_VALIDATORS, ValidatorFn} from "@angular/forms";

@Directive({
  selector: '[emailValidator][ngModel]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: EmailValidatorDirective,
      multi: true
    }
  ]
})
export class EmailValidatorDirective {

  validator: ValidatorFn;

  constructor() {
    this.validator = this.emailValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  emailValidator(): ValidatorFn {
    return (c: FormControl) => {
      let isValid = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(c.value);
      if (isValid) {
        return null;
      } else {
        return {
          emailValidator: {
            valid: false
          }
        };
      }
    }
  }

}

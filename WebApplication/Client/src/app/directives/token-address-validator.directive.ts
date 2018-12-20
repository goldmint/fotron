import { Directive } from '@angular/core';
import {FormControl, NG_VALIDATORS, Validator, ValidatorFn} from '@angular/forms';
import {TronService} from "../services/tron.service";

@Directive({
  selector: '[tokenAddressValidator][ngModel]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: TokenAddressValidatorDirective,
      multi: true
    }
  ]
})
export class TokenAddressValidatorDirective implements Validator {

  validator: ValidatorFn;

  constructor(
    private tronService: TronService,
  ) {
    this.validator = this.addressValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  addressValidator(): ValidatorFn {
    return (c: FormControl) => {
      let isValid = this.tronService.isValidAddress(c.value);
      if (isValid) {
        return null;
      } else {
        return {
          addressValidator: {
            valid: false
          }
        };
      }
    }
  }

}

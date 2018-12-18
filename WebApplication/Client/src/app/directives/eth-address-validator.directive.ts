import { Directive } from '@angular/core';
import {FormControl, NG_VALIDATORS, Validator, ValidatorFn} from '@angular/forms';
import {EthereumService} from "../services/ethereum.service";

@Directive({
  selector: '[ethAddressValidator][ngModel]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: EthAddressValidatorDirective,
      multi: true
    }
  ]
})
export class EthAddressValidatorDirective implements Validator {

  validator: ValidatorFn;

  constructor(
    private ethService: EthereumService,
  ) {
    this.validator = this.addressValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  addressValidator(): ValidatorFn {
    return (c: FormControl) => {
      let isValid = this.ethService.isValidAddress(c.value);
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

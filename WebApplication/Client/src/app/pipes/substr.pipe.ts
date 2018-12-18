import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'substr'})
export class SubstrPipe implements PipeTransform {
  transform(value: number, digits: number) {
    const position = value.toString().indexOf('.');
    if (position >= 0) {
      return value.toString().substr(0, position + digits + 1);
    } else {
      return value;
    }
  }
}
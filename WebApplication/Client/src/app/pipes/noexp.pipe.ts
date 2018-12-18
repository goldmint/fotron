import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'noexp'})
export class NoexpPipe implements PipeTransform {
  transform(value: number, pos: number = 4) {
    const amount = this.getNoExpValue(value);
    const position = amount.toString().indexOf('.');
    if (position >= 0) {
      let amountStr = amount.toString().substr(0, position + pos+1);
      pos >= 0 && (amountStr = amountStr.replace(/0+$/, '0'));
      if (+amountStr === 0) {
        return 0;
      } else {
        return amountStr;
      }
    } else {
      return amount;
    }
  }

  getNoExpValue(value) {
    let data = String(value).split(/[eE]/);
    if(data.length== 1) return data[0];

    let z= '', sign = value<0? '-':'',
      str= data[0].replace('.', ''),
      mag= Number(data[1])+ 1;

    if(mag<0){
      z= sign + '0.';
      while(mag++) z += '0';
      return z + str.replace(/^\-/,'');
    }
    mag -= str.length;
    while(mag--) z += '0';
    return str + z;
  }
}
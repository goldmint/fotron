import {ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {BigNumber} from "bignumber.js";
import {CommonService} from "./services/common.service";
import {MessageBoxService} from "./services/message-box.service";
import {TronService} from "./services/tron..service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {

  public ethBalance: BigNumber = null;
  public tokenBalance: BigNumber | any = null;
  public ethAddress: string = null;

  constructor(
    private tronService: TronService,
    private commonService: CommonService,
    private messageBox: MessageBoxService,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    let modalSessionKeyName = 'main_modal_shown',
        modalSessionValue = window.sessionStorage.getItem(modalSessionKeyName);

    /*modalSessionValue === null && */this.messageBox.mainModal();
    window.sessionStorage.setItem(modalSessionKeyName, 'true');

    this.tronService.getObservableEthBalance().subscribe(balance => {
      if (balance !== null && (this.ethBalance === null || !this.ethBalance.eq(balance))) {
        this.ethBalance = balance;
        this.tronService.passEthBalance.next(balance);
      }
    });

    this.tronService.getObservableTokenBalance().subscribe((balance) => {
      if (balance !== null && (this.tokenBalance === null || !this.tokenBalance.eq(balance))) {
        this.tokenBalance = balance;
        this.tronService.passTokenBalance.next(balance);
      }
    });

    this.tronService.getObservableEthAddress().subscribe(ethAddr => {
      if (ethAddr && this.tronService._contractInfura) {
        this.tokenBalance && this.tronService.passTokenBalance.next(this.tokenBalance);
        this.ethBalance && this.tronService.passEthBalance.next(this.ethBalance);
      }

      this.ethAddress = ethAddr;
      this.tronService.passEthAddress.next(ethAddr);
      this.cdRef.markForCheck();
    });
  }

}

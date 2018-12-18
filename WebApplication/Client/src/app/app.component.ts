import {ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {EthereumService} from "./services/ethereum.service";
import {BigNumber} from "bignumber.js";
import {CommonService} from "./services/common.service";
import {MessageBoxService} from "./services/message-box.service";

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
    private ethService: EthereumService,
    private commonService: CommonService,
    private messageBox: MessageBoxService,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    let modalSessionKeyName = 'main_modal_shown',
        modalSessionValue = window.sessionStorage.getItem(modalSessionKeyName);

    /*modalSessionValue === null && */this.messageBox.mainModal();
    window.sessionStorage.setItem(modalSessionKeyName, 'true');

    this.ethService.getObservableEthBalance().subscribe(balance => {
      if (balance !== null && (this.ethBalance === null || !this.ethBalance.eq(balance))) {
        this.ethBalance = balance;
        this.ethService.passEthBalance.next(balance);
      }
    });

    this.ethService.getObservableTokenBalance().subscribe((balance) => {
      if (balance !== null && (this.tokenBalance === null || !this.tokenBalance.eq(balance))) {
        this.tokenBalance = balance;
        this.ethService.passTokenBalance.next(balance);
      }
    });

    this.ethService.getObservableEthAddress().subscribe(ethAddr => {
      if (ethAddr && this.ethService._contractInfura) {
        this.tokenBalance && this.ethService.passTokenBalance.next(this.tokenBalance);
        this.ethBalance && this.ethService.passEthBalance.next(this.ethBalance);
      }

      this.ethAddress = ethAddr;
      this.ethService.passEthAddress.next(ethAddr);
      this.cdRef.markForCheck();
    });
  }

}

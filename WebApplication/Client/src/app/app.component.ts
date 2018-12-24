import {ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {CommonService} from "./services/common.service";
import {MessageBoxService} from "./services/message-box.service";
import {TronService} from "./services/tron.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {

  public trxBalance: number = null;
  public tokenBalance: number = null;
  public trxAddress: string = null;

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

    this.tronService.getObservableTrxBalance().subscribe(balance => {
      if (balance !== null && (this.trxBalance === null || this.trxBalance !== balance)) {
        this.trxBalance = balance;
        this.tronService.passTrxBalance.next(balance);
      }
    });

    this.tronService.getObservableTokenBalance().subscribe((balance) => {
      if (balance !== null && (this.tokenBalance === null || this.tokenBalance !== balance)) {
        this.tokenBalance = balance;
        this.tronService.passTokenBalance.next(balance);
      }
    });

    this.tronService.getObservableTronAddress().subscribe(address => {
      if (address && this.tronService.fotronContract) {
        this.tokenBalance && this.tronService.passTokenBalance.next(this.tokenBalance);
        this.trxBalance && this.tronService.passTrxBalance.next(this.trxBalance);
      }

      this.trxAddress = address;
      this.tronService.passTrxAddress.next(address);
      this.cdRef.markForCheck();
    });
  }

}

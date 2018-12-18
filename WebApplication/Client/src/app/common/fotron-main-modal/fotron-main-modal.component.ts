import {AfterViewInit, ChangeDetectionStrategy, Component, OnInit, ViewEncapsulation} from '@angular/core';
import {BsModalRef} from "ngx-bootstrap/modal";

@Component({
  selector: 'app-fotron-main-modal',
  templateUrl: './fotron-main-modal.component.html',
  styleUrls: ['./fotron-main-modal.component.sass'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.Default
})
export class FotronMainModalComponent implements OnInit, AfterViewInit {

  public slides = new Array(4);
  public currentSlide: number = 0;

  constructor(
    private bsModalRef: BsModalRef
  ) { }

  ngOnInit() { }

  randomInteger(min, max) {
    let rand = min + Math.random() * (max + 1 - min);
    rand = Math.floor(rand);
    return rand;
  }

  public hide() {
    this.bsModalRef.hide();
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.currentSlide = this.randomInteger(0, this.slides.length - 1);
    }, 0)
  }

}

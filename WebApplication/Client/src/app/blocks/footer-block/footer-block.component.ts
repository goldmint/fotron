import { Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './footer-block.component.html',
  styleUrls: ['./footer-block.component.sass'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterBlockComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}

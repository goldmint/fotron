import {ChangeDetectionStrategy, ChangeDetectorRef, Component, HostBinding, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Subject} from "rxjs/Subject";
import {document} from "ngx-bootstrap/utils/facade/browser";

@Component({
  selector: 'app-faq',
  templateUrl: './faq.component.html',
  styleUrls: ['./faq.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FaqComponent implements OnInit, OnDestroy {

  @HostBinding('class') class = 'page';

  public collapse: any = {};
  public totalCollapses = {
    about: 10,
    faq: 12
  }
  public ngForArray = new Array(this.totalCollapses.faq);

  private destroy$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private route: ActivatedRoute,
    private cdRef: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.route.params.takeUntil(this.destroy$).subscribe(params => {
      for (let i = 1; i <= (this.totalCollapses.about + this.totalCollapses.faq); i++) {
        this.collapse['item' + i] = true;
      }

      if (params.id && this.collapse['item' + params.id]) {
        this.collapse['item' + params.id] = false;
        this.cdRef.markForCheck();

       setTimeout(() => {
         let element = document.querySelector('.collapse-heading.open');
         element && element.scrollIntoView();
       }, 0);
      }

    });
  }

  ngOnDestroy() {
    this.destroy$.next(true);
  }

}

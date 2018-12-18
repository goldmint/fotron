import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuySellModalComponent } from './buy-sell-modal.component';

describe('BuySellModalComponent', () => {
  let component: BuySellModalComponent;
  let fixture: ComponentFixture<BuySellModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuySellModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuySellModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

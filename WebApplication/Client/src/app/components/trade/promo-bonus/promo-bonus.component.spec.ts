import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PromoBonusComponent } from './promo-bonus.component';

describe('PromoBonusComponent', () => {
  let component: PromoBonusComponent;
  let fixture: ComponentFixture<PromoBonusComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PromoBonusComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PromoBonusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

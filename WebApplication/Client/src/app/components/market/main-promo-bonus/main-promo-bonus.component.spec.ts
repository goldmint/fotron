import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MainPromoBonusComponent } from './main-promo-bonus.component';

describe('MainPromoBonusComponent', () => {
  let component: MainPromoBonusComponent;
  let fixture: ComponentFixture<MainPromoBonusComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MainPromoBonusComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MainPromoBonusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

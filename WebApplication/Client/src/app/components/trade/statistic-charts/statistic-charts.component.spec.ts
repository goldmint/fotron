import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StatisticChartsComponent } from './statistic-charts.component';

describe('StatisticChartsComponent', () => {
  let component: StatisticChartsComponent;
  let fixture: ComponentFixture<StatisticChartsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StatisticChartsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatisticChartsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

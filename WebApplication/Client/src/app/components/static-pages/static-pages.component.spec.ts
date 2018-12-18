import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaticPagesComponent } from './static-pages.component';

describe('StaticPagesComponent', () => {
  let component: StaticPagesComponent;
  let fixture: ComponentFixture<StaticPagesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StaticPagesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaticPagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

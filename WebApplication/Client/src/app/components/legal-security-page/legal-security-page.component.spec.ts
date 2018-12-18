import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegalSecurityPageComponent } from './legal-security-page.component';

describe('LegalSecurityPageComponent', () => {
  let component: LegalSecurityPageComponent;
  let fixture: ComponentFixture<LegalSecurityPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegalSecurityPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalSecurityPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

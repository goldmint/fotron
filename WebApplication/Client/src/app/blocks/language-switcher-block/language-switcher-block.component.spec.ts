import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LanguageSwitcherBlockComponent } from './language-switcher-block.component';

describe('LanguageSwitcherBlockComponent', () => {
  let component: LanguageSwitcherBlockComponent;
  let fixture: ComponentFixture<LanguageSwitcherBlockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LanguageSwitcherBlockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LanguageSwitcherBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

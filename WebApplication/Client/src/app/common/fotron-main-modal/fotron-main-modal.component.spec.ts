import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FotronMainModalComponent } from './fotron-main-modal.component';

describe('FotronMainModalComponent', () => {
  let component: FotronMainModalComponent;
  let fixture: ComponentFixture<FotronMainModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FotronMainModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FotronMainModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

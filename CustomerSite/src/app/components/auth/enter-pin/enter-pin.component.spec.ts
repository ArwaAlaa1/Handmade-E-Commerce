import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EnterPinComponent } from './enter-pin.component';

describe('EnterPinComponent', () => {
  let component: EnterPinComponent;
  let fixture: ComponentFixture<EnterPinComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EnterPinComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EnterPinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

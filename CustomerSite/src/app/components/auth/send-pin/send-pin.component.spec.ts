import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SendPinComponent } from './send-pin.component';

describe('SendPinComponent', () => {
  let component: SendPinComponent;
  let fixture: ComponentFixture<SendPinComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SendPinComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SendPinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

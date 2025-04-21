import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetAllAddressesComponent } from './get-all-addresses.component';

describe('GetAllAddressesComponent', () => {
  let component: GetAllAddressesComponent;
  let fixture: ComponentFixture<GetAllAddressesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetAllAddressesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GetAllAddressesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

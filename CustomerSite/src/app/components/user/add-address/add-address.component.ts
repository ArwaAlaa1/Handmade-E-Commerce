import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../services/user.service';
import { Router, RouterLink } from '@angular/router';
import { ShippingService } from '../../../services/shipping.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-add-address',
  imports: [ReactiveFormsModule,CommonModule, RouterLink],
  templateUrl: './add-address.component.html',
  styleUrl: './add-address.component.css'
})
export class AddAddressComponent implements OnInit {
  addAddressForm: FormGroup = new FormGroup({
    fullName: new FormControl('', Validators.required),
    phoneNumber: new FormControl('', [Validators.required]),
    region: new FormControl('', [Validators.required]),
    city: new FormControl('', [Validators.required]),
    country: new FormControl(null, { validators: [Validators.required] }),
    addressDetails: new FormControl(null, { validators: [Validators.required]}),
  });

  errorMessage: string = '';
  isLoading: boolean = false;
  shipingAddresses: any[] = [];

  constructor(private _userService: UserService, private _router : Router, private shipingService: ShippingService) {
  }

  ngOnInit(): void {
    this.getAddress();
  }

  getAddress(){
    this.isLoading = true;
    this.shipingService.getShippingCosts(
    ).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (response) => {
        this.shipingAddresses = response;
      },
      error: (error) => {
      }
    });
  }

  get f() {
    return this.addAddressForm.controls;
  }

  onSubmit(): void {
    if (this.addAddressForm.invalid) {
      this.addAddressForm.markAllAsTouched();
      return;
    }
    this.addAddress();
  }

  private addAddress(): void {
    this.isLoading = true;
    const formValue = this.addAddressForm.value;

    this._userService.AddAddress(formValue).subscribe({
      next: (response) => {
        this.isLoading = false;
        window.alert('Address added successfully!');
        this._router.navigate([`/profile`])
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
      }
    });
  }
}

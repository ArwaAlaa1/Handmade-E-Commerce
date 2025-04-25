import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-address',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './add-address.component.html',
  styleUrl: './add-address.component.css'
})
export class AddAddressComponent {
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

    constructor(private _userService: UserService, private _router : Router) {
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

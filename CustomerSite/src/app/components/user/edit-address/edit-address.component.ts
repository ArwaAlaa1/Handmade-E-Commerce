import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../services/user.service';
import { ActivatedRoute } from '@angular/router';

@Component({

  selector: 'app-edit-address',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './edit-address.component.html',
  styleUrl: './edit-address.component.css'
})
export class EditAddressComponent implements OnInit {
  editAddressForm: FormGroup = new FormGroup({
    fullName: new FormControl('', Validators.required),
    phoneNumber: new FormControl('', [Validators.required]),
    region: new FormControl('', [Validators.required]),
    city: new FormControl('', [Validators.required]),
    country: new FormControl(null, { validators: [Validators.required] }),
    addressDetails: new FormControl(null, { validators: [Validators.required]}),
  });

  errorMessage: string = '';
  isLoading: boolean = false;
  addressdata: any = {};
  Id: number = 0;

  constructor(private _userService: UserService, private _route : ActivatedRoute) {
    this.Id = this._route.snapshot.params['id'];
  }

  ngOnInit(): void {
    this._userService.getAddress(this.Id).subscribe({
      next: (addressdata) => {
        this.editAddressForm.patchValue({
          fullName: addressdata.fullName,
          phoneNumber: addressdata.phoneNumber,
          region: addressdata.region,
          country: addressdata.country,
          city: addressdata.city,
          addressDetails: addressdata.addressDetails
        });
      },
      error: (error) => {
        console.error('Error loading data:', error);
      }
    });
  }


  get f() {
    return this.editAddressForm.controls;
  }

  onSubmit(): void {
    if (this.editAddressForm.invalid) {
      this.editAddressForm.markAllAsTouched();
      return;
    }
    this.editAddress();
  }

  private editAddress(): void {
    this.isLoading = true;
    const formValue = this.editAddressForm.value;

    this._userService.EditAddress( this.Id,formValue).subscribe({
      next: (response) => {
        this.isLoading = false;

        window.alert('Address updated successfully!');
        window.location.href = '/profile';
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
      }
    });
  }
}

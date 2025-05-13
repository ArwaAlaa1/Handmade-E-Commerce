import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../services/user.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ShippingService } from '../../../services/shipping.service';
import { finalize } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-edit-address',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './edit-address.component.html',
  styleUrl: './edit-address.component.css'
})
export class EditAddressComponent implements OnInit {
  addressdata: any = {};
  editAddressForm: FormGroup;

  errorMessage: string = '';
  isLoading: boolean = false;
  shipingAddresses: any[] = [];
  Id: number = 0;

  constructor(private _userService: UserService, private _route: ActivatedRoute, private _Router: Router, private shipingService: ShippingService) {
    this.Id = this._route.snapshot.params['id'];
    this.editAddressForm = new FormGroup({
      fullName: new FormControl('', Validators.required),
      phoneNumber: new FormControl('', [Validators.required]),
      region: new FormControl('', [Validators.required]),
      city: new FormControl('', [Validators.required]),
      country: new FormControl('', { validators: [Validators.required] }),
      addressDetails: new FormControl('', { validators: [Validators.required] }),
    });
  }

  ngOnInit(): void {
    this._userService.getAddress(this.Id).subscribe({
      next: (addressdata) => {
        this.addressdata = addressdata;
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

    this.getAddress();
  }

  getAddress() {
    this.isLoading = true;
    this.shipingService.getShippingCosts().subscribe({
      next: (response) => {
        this.shipingAddresses = response;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading shipping addresses:', error);
        this.isLoading = false;
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

  // private editAddress(): void {
  //   this.isLoading = true;
  //   const formValue = this.editAddressForm.value;

  //   this._userService.EditAddress(this.Id, formValue).subscribe({
  //     next: (response) => {
  //       this.isLoading = false;
  //       window.alert('Address updated successfully!');
  //       this._Router.navigate(['/profile']);
  //     },
  //     error: (error) => {
  //       this.isLoading = false;
  //       this.errorMessage = error.error.message;
  //     }
  //   });
  // }

  private editAddress(): void {
    this.isLoading = true;
    const formValue = this.editAddressForm.value;

    this._userService.EditAddress(this.Id, formValue).subscribe({
      next: (response) => {
        this.isLoading = false;
        Swal.fire({
          title: 'Success!',
          text: 'Address updated successfully!',
          icon: 'success',
          confirmButtonColor: '#6c7fd8', 
          timer: 1500, 
          showConfirmButton: false 
        }).then(() => {
          this._Router.navigate(['/profile']);
        });
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
        Swal.fire({
          title: 'Error!',
          text: this.errorMessage || 'Failed to update address. Please try again.',
          icon: 'error',
          confirmButtonColor: '#e63946',
          confirmButtonText: 'OK'
        });
      }
    });
  }
}


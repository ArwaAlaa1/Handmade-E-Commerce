import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { UserService } from '../../../services/user.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-edit-profile',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.css'
})
export class EditProfileComponent implements OnInit {

  editForm: FormGroup = new FormGroup(
    {
      userName: new FormControl(null, { validators: [Validators.required] }),
      phone: new FormControl(null, { validators: [Validators.required] }),
    },
  );

  errorMessage: string = '';
  isLoading: boolean = false;
  userData: any = {};

  constructor(private _userService: UserService, private _Router : Router) {
  }

    ngOnInit(): void {
      this._userService.getUserProfile().subscribe({
        next: (user) => {
          this.editForm.patchValue({
            userName: user.userName,
            phone: user.phoneNumber,
          });
        },
        error: (error) => {
          console.error('Error loading data:', error);
        }
      });
    }

  get f() {
    return this.editForm.controls;
  }

  onSubmit() {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }
    this.submitEditProfileForm();
  }

  // submitEditProfileForm() {
  //   this.isLoading = true;
  //   const loginData = this.editForm.value;

  //   this._userService.EditProfile(loginData).subscribe({
  //     next: (response) => {
  //       this.isLoading = false;
  //       window.alert('Profile updated successfully');
  //       this._Router.navigate(['/profile']);
  //     },
  //     error: (error) => {
  //       this.isLoading = false;
  //       this.errorMessage = error.error.message;
  //     }
  //   });
  // }

  submitEditProfileForm() {
    this.isLoading = true;
    const loginData = this.editForm.value;

    this._userService.EditProfile(loginData).subscribe({
      next: (response) => {
        this.isLoading = false;
        Swal.fire({
          title: 'Success!',
          text: 'Profile updated successfully',
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
          text: this.errorMessage || 'Failed to update profile. Please try again.',
          icon: 'error',
          confirmButtonColor: '#e63946',
          confirmButtonText: 'OK'
        });
      }
    });
  }
}


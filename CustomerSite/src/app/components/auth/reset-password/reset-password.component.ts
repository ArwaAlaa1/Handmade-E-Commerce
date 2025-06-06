import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { CookieService } from 'ngx-cookie-service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-forget-password',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {

  forgetPasswordForm: FormGroup = new FormGroup(
    {
      newPassword: new FormControl(null, { validators: [Validators.required, Validators.minLength(8), this.passwordStrengthValidator()] }),
      confirmNewPassword: new FormControl(null, { validators: [Validators.required] }),
    },
    { validators: this.passwordMatchValidator }
  );

  isLoading: boolean = false;
  errorMessage: string = '';
  email: string = '';


  passwordVisible: boolean = false;
  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }

  confirmPasswordVisible: boolean = false;
  toggleConfirmPasswordVisibility() {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }

  constructor(private _authService: AuthService, private _CookieService: CookieService,
    private _Router: Router, private _route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.email = this._route.snapshot.params['email'];
  }

  get f() {
    return this.forgetPasswordForm.controls;
  }


  passwordStrengthValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value || '';
      const hasLowerCase = /[a-z]/.test(value);
      const hasUpperCase = /[A-Z]/.test(value);
      const hasDigit = /[0-9]/.test(value);
      const hasSpecialChar = /[@%$#]/.test(value);

      return hasLowerCase && hasUpperCase && hasDigit && hasSpecialChar ? null : { passwordStrength: true };
    };
  }

  passwordMatchValidator(formGroup: AbstractControl): ValidationErrors | null {
    const newPassword = formGroup.get('newPassword')?.value;
    const confirmNewPassword = formGroup.get('confirmNewPassword')?.value;
    return newPassword === confirmNewPassword ? null : { passwordMismatch: true };
  }


  onSubmit() {
    if (this.forgetPasswordForm.invalid) {
      this.forgetPasswordForm.markAllAsTouched();
      return;
    }
    this.submitVerifyForm();
  }

  submitVerifyForm() {
    this.isLoading = true;
    const pinData = {
      newPassword: this.forgetPasswordForm.value.newPassword,
      confirmNewPassword: this.forgetPasswordForm.value.confirmNewPassword,
    };

    const pass = this._CookieService.get('pass');
    if (!pass || pass == 'false') {
      this._Router.navigate([`/sendpin`]);
      // Replace window.alert with SweetAlert2
      Swal.fire({
        icon: 'warning',
        title: 'Action Required',
        text: 'Please send a pin code before changing your password.',
        confirmButtonText: 'OK',
        confirmButtonColor: '#ffc107' // Yellow for warning
      }).then(() => {
        this._Router.navigate([`/sendpin`]);
      });
      return;
    }
    this._authService.ForgetPassword(this.email, pinData).subscribe({
      next: (response) => {
        this.isLoading = false;
        // Replace window.alert with SweetAlert2
        Swal.fire({
          icon: 'success',
          title: 'Password Changed!',
          text: 'Your password has been changed successfully. Login now!',
          confirmButtonText: 'OK',
          confirmButtonColor: '#28a745'
        }).then(() => {
          if (!pass) {
            this._CookieService.set('pass', JSON.stringify(false), {
              expires: 1,
              path: '/',
            });
          } else {
            this._CookieService.delete('pass', '/');
            this._CookieService.set('pass', JSON.stringify(false), {
              expires: 1,
              path: '/',
            });
          }
          this._Router.navigate([`/login`]);
        });
      },
      error: (error) => {
        this.isLoading = false;
        if (error.status === 404 || error.status === 400) {
          this.errorMessage = error.errorMessage || 'An error occurred.';
          // Show error with SweetAlert2
          Swal.fire({
            icon: 'error',
            title: 'Error',
            text: this.errorMessage,
            confirmButtonText: 'OK',
            confirmButtonColor: '#dc3545'
          });
        }
      }
    });
  }
}

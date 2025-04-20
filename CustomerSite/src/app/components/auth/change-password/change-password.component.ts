import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { log } from 'console';

@Component({
  selector: 'app-change-password',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent implements OnInit {

  changePasswordForm: FormGroup = new FormGroup(
    {
      oldPassword: new FormControl(null, { validators: [Validators.required]}),
      newPassword: new FormControl(null, { validators: [Validators.required, Validators.minLength(8), this.passwordStrengthValidator()] }),
      confirmPassword: new FormControl(null, { validators: [Validators.required]}),
    },
    { validators: this.passwordMatchValidator }
  );

  errorMessage: string = '';
  isLoading :boolean = false;
  email: string = '';

  oldPasswordVisible: boolean = false;
  toggleOldPasswordVisibility() {
    this.oldPasswordVisible = !this.oldPasswordVisible;
  }

  passwordVisible: boolean = false;
  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }

  confirmPasswordVisible: boolean = false;
  toggleConfirmPasswordVisibility() {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }

  constructor(private _authService: AuthService,
    private _Router : Router, private _route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.email = this._route.snapshot.params['email'];
  }

  get f() {
    return this.changePasswordForm.controls;
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
    const confirmPassword = formGroup.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { passwordMismatch: true };
  }


  onSubmit() {
    if (this.changePasswordForm.invalid) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }
    this.submitVerifyForm();
  }

  submitVerifyForm() {
    this.isLoading = true;
    const changedData = this.changePasswordForm.value;

    this._authService.ChangePassword(changedData).subscribe({
      next: (response) => {
        this.isLoading = false;
        window.alert(`${response.message}`);
        // this._Router.navigate([`/home`]);
        window.location.href = '/home';
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
      }
    });
  }

}

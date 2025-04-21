import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  RegisterForm: FormGroup = new FormGroup({
    displayName: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    userName: new FormControl('', [Validators.required, Validators.pattern(/^\S+$/)]),
    phoneNumber: new FormControl('', [Validators.required, Validators.pattern(/^\d{11}$/)]),
    password: new FormControl(null, { validators: [Validators.required, Validators.minLength(8), this.passwordStrengthValidator()] }),
    confirmPassword: new FormControl(null, { validators: [Validators.required]}),
  },
  { validators: this.passwordMatchValidator });


  errorMessage: string = '';
  isLoading: boolean = false;
  passwordVisible: boolean = false;
  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }
  confirmPasswordVisible: boolean = false;
  toggleConfirmPasswordVisibility() {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }

    constructor(private authService: AuthService) {
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
    const password = formGroup.get('Password')?.value;
    const confirmPassword = formGroup.get('ConfirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  get f() {
    return this.RegisterForm.controls;
  }

  onSubmit(): void {
    if (this.RegisterForm.invalid) {
      this.RegisterForm.markAllAsTouched();
      return;
    }
    this.register();
  }

  private register(): void {
    this.isLoading = true;
    const formValue = this.RegisterForm.value;

    this.authService.signup(formValue).subscribe({
      next: () => {
        this.isLoading = false;
        window.alert('Registration Successful! Please confirm your email before logging in.');
        window.location.href = '/login';
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
      }
    });
  }
}

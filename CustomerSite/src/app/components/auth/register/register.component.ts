import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';
import { log } from 'node:console';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  RegisterForm: FormGroup;
  submitted = false;

  constructor(private authService: AuthService) {
    this.RegisterForm = new FormGroup({
      displayName: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      userName: new FormControl('', [Validators.required, Validators.pattern(/^\S+$/)]),
      phonenumber: new FormControl('', [Validators.required, Validators.pattern(/^\d{11}$/)]),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
      confirmpassword: new FormControl('', Validators.required)
    }, { validators: this.matchPasswords('password', 'confirmpassword') as ValidatorFn });
  }

  matchPasswords(passwordKey: string, confirmPasswordKey: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const group = control as FormGroup;
      const password = group.controls[passwordKey];
      const confirmPassword = group.controls[confirmPasswordKey];
      if (password.value !== confirmPassword.value) {
        confirmPassword.setErrors({ notEquivalent: true });
        return { notEquivalent: true };
      } else {
        confirmPassword.setErrors(null);
        return null;
      }
    };
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.RegisterForm.invalid) {
     console.log('Form is invalid');
      this.RegisterForm.markAllAsTouched();
      return;

    }

    const formValue = this.RegisterForm.value;

    // Remove confirmPassword (not needed for backend)
    const requestData = {
      displayName: formValue.displayName,
      userName: formValue.userName,
      phoneNumber: formValue.phonenumber,
      email: formValue.email,
      password: formValue.password,
      confirmPassword: formValue.confirmpassword // Not needed for backend
    };

    this.authService.signup(requestData).subscribe({
      next: (res) => {
        console.log('Registration successful:', res);
        // You can navigate to login page or show success message here
      },
      error: (err) => {
        console.error(' Registration error:', err);
      }
    });
  }
}

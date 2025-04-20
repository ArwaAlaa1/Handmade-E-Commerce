import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup = new FormGroup(
    {
      emailOrUserName: new FormControl(null, { validators: [Validators.required, Validators.email] }),
      password: new FormControl(null, { validators: [Validators.required] }),
    },
  );

  errorMessage: string = '';
  isLoading: boolean = false;
  passwordVisible: boolean = false;

  constructor(private _authService: AuthService, private _router: Router) {
  }

  ngOnInit(): void {}

  get f() {
    return this.loginForm.controls;
  }

  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    this.loginUser();
  }

  private loginUser(): void {
    this.isLoading = true;
    const loginData = this.loginForm.value;

    this._authService.login(loginData).subscribe({
      next: (response) => {
        this._authService.saveUserData(response);
        this.isLoading = false;
        // this._router.navigate(['/home']);
        window.location.href = '/home';
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
      }
    });
  }
}

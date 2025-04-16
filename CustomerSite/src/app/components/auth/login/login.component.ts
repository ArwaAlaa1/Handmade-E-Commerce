// import { Component } from '@angular/core';
// @Component({
//   selector: 'app-login',
//   imports: [],
//   templateUrl: './login.component.html',
//   styleUrl: './login.component.css'
// })
// export class LoginComponent {
// }


// import { CommonModule } from '@angular/common';
// import { Component, OnInit } from '@angular/core';
// import { ReactiveFormsModule, FormGroup, FormControl, Validators} from '@angular/forms';
// import { AuthService } from '../../../services/auth.service';
// import { Router, RouterLink } from '@angular/router';

// @Component({
//   selector: 'app-login',
//   imports: [ReactiveFormsModule, CommonModule,RouterLink],
//   templateUrl: './login.component.html',
//   styleUrl: './login.component.css'
// })

// export class LoginComponent implements OnInit{
//   loginForm: FormGroup = new FormGroup(
//     {
//       email: new FormControl(null, { validators: [Validators.required, Validators.email] }),
//       password: new FormControl(null, { validators: [Validators.required] }),
//     },
//   );

//   errorMessage: string = '';
//   isLoading :boolean = false;

//   passwordVisible: boolean = false;
//   togglePasswordVisibility() {
//     this.passwordVisible = !this.passwordVisible;
//   }

//   constructor(private _authService: AuthService,
//     private _Router : Router
//   ) {}

//   ngOnInit(): void {}

//   get f() {
//     return this.loginForm.controls;
//   }

//   onSubmit() {
//     if (this.loginForm.invalid) {
//       this.loginForm.markAllAsTouched();
//       return;
//     }
//     this.submitLoginForm();
//   }

//   submitLoginForm() {
//     this.isLoading = true;
//     const loginData = {
//       email: this.loginForm.value.email,
//       password: this.loginForm.value.password,
//     };

//     this._authService.login(loginData).subscribe({
//       next: (response) => {
//         this._authService.saveUserData(response);
//         this.isLoading = false;
//         this._Router.navigate(['/home']);
//       },
//       error: (error) => {
//         this.isLoading = false;
//         this.errorMessage = 'invalid email or password';
//       }
//     });
//   }
// }

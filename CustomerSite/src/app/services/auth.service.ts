// import { Injectable, inject } from '@angular/core';
// import { HttpClient, HttpHeaders } from '@angular/common/http';
// import { Observable, BehaviorSubject } from 'rxjs';
// import { Router } from '@angular/router';
// import { CookieService } from 'ngx-cookie-service';
// import { environment } from '../../environments/environment.development';
// import { jwtDecode } from 'jwt-decode';

// @Injectable({
//   providedIn: 'root',
// })
// export class AuthService {

//   userData = new BehaviorSubject<any>(null);

//   constructor(private _HttpClient: HttpClient, private _Router: Router, private _CookieService: CookieService)
//   {
//     this.loadUserData();
//   }

//   private isValidToken(token: string): boolean {
//     if (!token) return false;
//     try {
//       const decoded: any = jwtDecode(token);
//       return decoded.exp * 1000 > Date.now();
//     } catch (error) {
//       return false;
//     }
//   }

//   saveUserData(response: any) {
//     const userData = {
//       token: response.token,
//       userId: response.userId,
//       userName: response.userName,
//       email: response.email,
//       image: response.image,
//       decodedToken: jwtDecode(response.token),
//     };

//     this._CookieService.set('userData', JSON.stringify(userData), {
//       expires: 7,
//       path: '/',
//     });

//     this.userData.next(userData);
//   }

//   loadUserData() {
//     const storedData = this._CookieService.get('userData');
//     if (storedData) {
//       const parsedData = JSON.parse(storedData);
//       if (this.isValidToken(parsedData.token)) {
//         this.userData.next(parsedData);
//       } else {
//         this.signout();
//       }
//     }
//   }

//   signout() {
//     this._CookieService.delete('userData', '/');
//     this.userData.next(null);
//     this._Router.navigate(['/login']);
//   }

//   signup(formData: any): Observable<any> {
//     return this._HttpClient.post(`${environment.baseURL}User/register`, formData);
//   }

//   login(credentials: { email: string; password: string }): Observable<any> {
//     const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
//     return this._HttpClient.post(`${environment.baseURL}User/login`, credentials, { headers });
//   }

//   isAuthenticated(): boolean {
//     const user = this.userData.value;
//     return !!user && this.isValidToken(user.token);
//   }
// }

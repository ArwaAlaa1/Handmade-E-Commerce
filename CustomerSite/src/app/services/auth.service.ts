import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  userData = new BehaviorSubject<any>(null);

  constructor(private _HttpClient: HttpClient, private _Router: Router, private _CookieService: CookieService)
  {
    this.loadUserData();
  }

  saveUserData(response: any) {
    const userData = {
      token: response.token,
      userId: response.userId,
      displayName: response.displayName,
      userName: response.userName,
      email: response.email,
      image: response.image,
    };

    this._CookieService.set('userData', JSON.stringify(userData), {
      expires: 7,
      path: '/',
    });

    this.userData.next(userData);
  }

  loadUserData() {
    const storedData = this._CookieService.get('userData');
    if (storedData) {
      const parsedData = JSON.parse(storedData);
      if (parsedData.token) {
        this.userData.next(parsedData);
      } else {
        this.signout();
      }
    }
  }

  private getAuthHeaders(): HttpHeaders {
    const storedData = this._CookieService.get('userData');
    if (!storedData) {
      return new HttpHeaders();
    }
    const parsedData = JSON.parse(storedData);
    if (parsedData && parsedData.token) {
      return new HttpHeaders({
        Authorization: `Bearer ${parsedData.token}`,
      });
    }
    return new HttpHeaders();
  }

  signout() {
    this._CookieService.delete('userData', '/');
    this.userData.next(null);
    // this._Router.navigate(['/home']);
    window.location.href = '/home';
  }

  signup(
    credentials:{
      displayName: string;
      userName: string;
      phoneNumber: string;
      email: string;
      password: string;
      confirmPassword: string;
    }): Observable<any>
    {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/register`, credentials, { headers });
  }

  login(credentials: { emailOrUserName: string; password: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/login`, credentials, { headers });
  }

  SendPinCode(credentials: { email: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/send_reset_code`, credentials, { headers });
  }

  Verify_Pin( email : string, credentials: { pin: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/verify_pin/${email}`, credentials, { headers });
  }

  ForgetPassword( email : string, credentials: { newPassword: string, confirmNewPassword: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/forget_password/${email}`, credentials, { headers });
  }

  ChangePassword( credentials: { oldPassword: string, newPassword: string, confirmPassword: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/change_password`, credentials, { headers: this.getAuthHeaders() });
  }


}

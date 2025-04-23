import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '../../environments/environment.development';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {

  email: string = '';

  constructor(private _HttpClient: HttpClient, private _auth: AuthService, private _CookieService: CookieService) {
    this._auth.userData.subscribe({
      next: (data) => {
        if (data) {
          this.email = data.email;
        }
      }
    });
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

  getUserProfile(): Observable<any>
  {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.get(`${environment.baseURL}User/GetUser`, { headers : this.getAuthHeaders() });
  }

  EditProfile( credentials: { userName: string, phone: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.put(`${environment.baseURL}User/UpdateUserData`, credentials, { headers : this.getAuthHeaders()});
  }

  AddImage(formData: any): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}User/AddUserImage`, formData ,
      { responseType: 'text', headers : this.getAuthHeaders() } );
  }

  AddAddress( credentials: { fullName: string, phoneNumber: string, region: string, city: string, country: string, addressDetails: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}User/AddAddress`, credentials, { headers: this.getAuthHeaders() });
  }

  EditAddress( id : number, credentials: { fullName: string, phoneNumber: string, region: string, city: string, country: string, addressDetails: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.put(`${environment.baseURL}User/UpdateAddress?id=${id}`, credentials, { headers : this.getAuthHeaders() });
  }

  getAddress(id: number): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.get(`${environment.baseURL}User/GetAddress?id=${id}`, { headers : this.getAuthHeaders() });
  }

}

import { Cart } from './../interfaces/cart';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { log } from 'console';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  token: string = '';
  private baseUrl = 'https://localhost:7223/api/Cart';
  isLogin: boolean = false;
  constructor(private http: HttpClient,private _cookie:CookieService,private _auth: AuthService) {
    // this.loadUserData();
    this._auth.userData.subscribe({
      next: (data) => {
        if (data) {
          this.token = data.token;
        }
      }
    });
  }
  public getAuthHeaders(): HttpHeaders {
    const storedData = this._cookie.get('userData');
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

 
  getCartById(): Observable<any>{
    // const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get(`${this.baseUrl}`, { headers:this.getAuthHeaders()});
  }

 updateCart(cart:Cart): Observable<any> {
    // const headers = new HttpHeaders().set('Authorization', `Bearer ${this.token}`);
    if  (this.token) {
      const headers = new HttpHeaders({ 'Content-Type': 'application/json','Authorization': `Bearer ${this.token}` });
      return this.http.post(`${this.baseUrl}/UpdateCart`,cart, { headers });
    }
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.baseUrl}/UpdateCart`,cart, { headers });
  }
 
}

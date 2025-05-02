import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  token: string = '';
  private baseUrl = 'https://localhost:7223/api/Order';
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
   getUserOrders(): Observable<any>{
    
      return this.http.get(`${this.baseUrl}/UserOrders`, { headers:this.getAuthHeaders()});
    }

    getOrder(orderid:number): Observable<any>{
    
      return this.http.get(`${this.baseUrl}/${orderid}`, { headers:this.getAuthHeaders()});
    }
    cancelOrder(orderid:number): Observable<any>{
    
      return this.http.get(`${this.baseUrl}/CancelOrder?orderId=${orderid}`, { headers:this.getAuthHeaders()});
    }
    cancelOrderItem(itemid:number): Observable<any>{
    
      return this.http.get(`${this.baseUrl}/CancelItem?orderItemId=${itemid}`, { headers:this.getAuthHeaders()});
    }
}

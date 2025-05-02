import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  token: string = '';
 
  isLogin: boolean = false;
  constructor(private http: HttpClient,private _cookie:CookieService,private _auth: AuthService) {
    this._auth.userData.subscribe({
         next: (data) => {
           if (data) {
             this.token = data.token;
           }
         }
       });
     }
   
     private getAuthHeaders(): HttpHeaders {
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
    
      return this.http.get(`${environment.baseURL}Order/UserOrders`, { headers:this.getAuthHeaders()});
    }

    getOrder(orderid:number): Observable<any>{
    
      return this.http.get(`${environment.baseURL}Order/${orderid}`, { headers:this.getAuthHeaders()});
    }
    cancelOrder(orderid:number): Observable<any>{
    
      return this.http.post(`${environment.baseURL}Order/CancelOrder?orderId=${orderid}`, { headers:this.getAuthHeaders()});
    }
    cancelOrderItem(itemid:number): Observable<any>{
      const headers = new HttpHeaders({ 'Content-Type': 'application/json','Authorization': `Bearer ${this.token}` });
      console.log("token",this.token);

      return this.http.post(`${environment.baseURL}Order/CancelItem?orderItemId=${itemid}`, { headers });
    }
}

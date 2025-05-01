import { HttpClient, HttpHeaders, provideHttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import e from 'express';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './auth.service';
import { Order } from '../interfaces/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl =environment.baseURL + '/Order';
  private token: string = '';
  constructor( private http: HttpClient,
    private cookieService: CookieService,
    private authService: AuthService) 
    {
      this.authService.userData.subscribe({
        next: (data: any) => {
          if (data?.token) this.token = data.token;
        }
      });
     }
     private getAuthHeaders(): HttpHeaders {
      const storedData = this.cookieService.get('userData');
      if (!storedData) return new HttpHeaders();
  
      try {
        const parsed = JSON.parse(storedData);
        if (parsed.token) {
          return new HttpHeaders({
            Authorization: `Bearer ${parsed.token}`
          });
        }
      } catch (e) {
        console.error('Failed to parse auth token:', e);
      }
  
      return new HttpHeaders();
    }
 


    getUserOrders(): Observable<any[]> {
      return this.http.get<any[]>(`${this.apiUrl}/UserOrders`, {
        headers: this.getAuthHeaders()
      });
    }
  // getOrderBtId(id: number) :Observable<any>{
  //   return this.http.get<any>(`${this.apiUrl}/${id}`)
  //   {
  //     headers: this.getAuthHeaders()
  //   };
  // }

  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

}
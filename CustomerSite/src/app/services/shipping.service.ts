import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShippingService {

  constructor(private http:HttpClient,private _cookie:CookieService) { }
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
  baseUrl = 'https://localhost:7223/api/ShippingCost';
   getShippingCosts(): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
      return this.http.get(`${this.baseUrl}`, { headers });
    }
}

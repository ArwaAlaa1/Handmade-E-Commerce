import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './auth.service';
import { Cart, CartItem } from '../interfaces/cart';
import { OrderResponse } from '../interfaces/order-response';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  token: string = '';
  private baseUrl =  environment.baseURL+ 'Payment';

  constructor(
    private http: HttpClient,
    private _cookie: CookieService,
    private _auth: AuthService
  ) {
    this._auth.userData.subscribe({
      next: (data: any) => {
        if (data && data.token) {
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

    let parsedData: any;
    try {
      parsedData = JSON.parse(storedData);
    } catch (e) {
      console.error('Error parsing cookie data:', e);
      return new HttpHeaders();
    }

    if (parsedData && parsedData.token) {
      return new HttpHeaders({
        Authorization: `Bearer ${parsedData.token}`,
      });
    }
    return new HttpHeaders();
  }

  createOrUpdatePayment(cardId: string, shippingCostId?: number): Observable<Cart> {
    const headers = this.getAuthHeaders();
    const params = new HttpParams()
      .set('cardId', cardId)
      .set('shippingCostId', shippingCostId?.toString() ?? '');

    console.log('Sending request with query params:', params.toString());


    return this.http.post<Cart>(
      `${this.baseUrl}/CreateOrUpdate`,
      null,
      { headers, params }
    );
  }

  createOrder(cartId: string, shippingCostId: number, addressId: number,paymentId: string): Observable<OrderResponse> {
    // const url = `https://localhost:7223/api/Order`;
    const url = `${environment.baseURL}Order`;

    const body = { cartId, shippingCostId, addressId, paymentId };
    return this.http.post<any>(url, body, { headers: this.getAuthHeaders() });
  }
}

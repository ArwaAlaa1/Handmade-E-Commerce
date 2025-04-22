import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  private baseUrl = 'https://localhost:7223/api/Cart';

  constructor(private http: HttpClient) { }

  getCartById(cartId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/${cartId}`);
  }
}

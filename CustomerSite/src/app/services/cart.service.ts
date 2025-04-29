import { Cart, CartItem } from './../interfaces/cart';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

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

  AddToCart(cart:Cart): Observable<any>{
    // const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    if  (this.token) {
      console.log('Token:', this.token); // Log the token to check if it's being set correctly
      console.log('Cart payload:', JSON.stringify(cart));

      // const headers = new HttpHeaders({ 'Content-Type': 'application/json','Authorization': `Bearer ${this.token}` });
      return this.http.post(`${this.baseUrl}`,cart, { headers:this.getAuthHeaders() });
    }
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.baseUrl}`,cart, { headers });
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
 
  addItemToBasket(newItem: CartItem, quantity = 1) {
    if (this.token !== '') {
      this.getCartById().subscribe({
        next: (res) => {
          const cartdata = res;
  
          console.log('Cart data received:', cartdata);
  
          // Ensure cartItems is initialized (defensive coding)
          cartdata.cartItems = cartdata.cartItems || [];
  
          const itemIndex = cartdata.cartItems.findIndex((item: CartItem) =>
            item.productId === newItem.productId &&
            item.color === newItem.color &&
            item.size === newItem.size
          );
  
          if (itemIndex !== -1) {
            
            cartdata.cartItems[itemIndex].quantity += quantity;
            console.log('Item quantity updated:', cartdata.cartItems[itemIndex]);
          } else {
            
            newItem.quantity = quantity; // Set quantity before pushing
            cartdata.cartItems.push(newItem);
            console.log('New item added:', newItem);
          }
  
          
          this.AddToCart(cartdata).subscribe({
            next: (res) => {
              console.log('Cart updated successfully:', res);
            },
            error: (err) => {
              console.error('Error adding item to cart:', err);
            }
          });
        },
        error: (err) => {
          console.error('Error fetching cart:', err);
        }
      });
    } else {
      console.warn('No token found — user might not be authenticated');
    }
  
    console.log('Item processed for cart:', newItem, 'Quantity:', quantity);
  }
  
  
}

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
    if (!this.token) {
      console.warn('No token found — user might not be authenticated');
      return;
    }
  
    this.getCartById().subscribe({
      next: (responseCart) => {
        const cart = responseCart;
        cart.cartItems = cart.cartItems || [];
  
        // Find if the same item already exists by productId, color, and size
        const itemIndex = cart.cartItems.findIndex((item: CartItem) =>
          item.productId === newItem.productId &&
          (item.color ?? null) === (newItem.color ?? null) &&
          (item.size ?? null) === (newItem.size ?? null)
        );
        
  
        if (itemIndex !== -1) {
          // Item exists, increase its quantity
          cart.cartItems[itemIndex].quantity += quantity;
          console.log('Item quantity updated:', cart.cartItems[itemIndex]);
  
          this.updateCart(cart).subscribe({
            next: (res) => console.log('Cart updated successfully:', res),
            error: (err) => console.error('Error updating cart:', err)
          });
        } else {
          // New item, add to cart with specified quantity
          newItem.quantity = quantity;
          cart.cartItems.push(newItem);
          console.log('New item added:', newItem);
  
          this.AddToCart(cart).subscribe({
            next: (res) => console.log('Cart updated successfully:', res),
            error: (err) => console.error('Error adding item to cart:', err)
          });
        }
  
        console.log('Item processed for cart:', newItem, 'Quantity:', quantity);
      },
      error: (err) => {
        console.error('Error fetching cart:', err);
      }
    });
  }
  
  
  
  
}

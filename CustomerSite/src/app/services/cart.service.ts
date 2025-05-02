import { Cart, CartItem } from './../interfaces/cart';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

import { CookieService } from 'ngx-cookie-service';
import { v4 as uuidv4 } from 'uuid';


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
     
      // const headers = new HttpHeaders({ 'Content-Type': 'application/json','Authorization': `Bearer ${this.token}` });
      return this.http.post(`${this.baseUrl}`,cart, { headers:this.getAuthHeaders() });
    }
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.baseUrl}`,cart, { headers });
  }
 

  getCartById(guestid?:string): Observable<any>{
    // const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    if  (this.token) {
      return this.http.get(`${this.baseUrl}`, { headers:this.getAuthHeaders()});
    }
    const headers = new HttpHeaders({ 'Content-Type': 'application/json'});
    return this.http.get(`${this.baseUrl}?cartId=${guestid}`, { headers});
  }
  getCarteById(guestid?:string): Observable<any>{
   
    const headers = new HttpHeaders({ 'Content-Type': 'application/json'});
    return this.http.get(`${this.baseUrl}?cartId=${guestid}`, { headers});
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
     
  const existingCartId = this._cookie.get('cartId');

  if (!existingCartId) {
    
    const uuid = uuidv4().toString();

   
    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + 7);
    this._cookie.set('cartId', uuid, expirationDate);

    const cart :Cart = {
      id: uuid,
      cartItems: [newItem]
    };
    // Create cart with this UUID
    this.AddToCart(cart).subscribe({
      next: (res) => {
         console.log('Cart updated successfully:', res);
      },
    
      error: err => {
        console.error('Failed to create cart:', err);
      }
    });
  }else{

    // If cartId exists, fetch the cart and add the item to it
    this.getCartById(existingCartId).subscribe({
      next: (responseCart) => {
        const cart = responseCart;
       
        cart.cartItems = cart.cartItems || [];
        console.log('Fetched cart:', cart); // Log the fetched cart for debugging
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
    }else{

      
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
  
  
  
  clearCart(id: string): Observable<void> {
    const headers = this.getAuthHeaders();
    const params = new HttpParams().set('id', id);
    return this.http.delete<void>(`${this.baseUrl}/DeleteCartAsync`, { headers, params });
 
  }
}
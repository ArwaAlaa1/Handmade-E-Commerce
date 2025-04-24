import { ChangeDetectorRef, Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';
import { Cart } from '../../interfaces/cart';

@Component({
  selector: 'app-cart',
  imports: [CommonModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  cartData: Cart= {} as Cart;
 token: string = '';
 userData: any = null;
  imageBaseUrl: string = `https://handmadee-commerce.runasp.net/images//`;
  isLogin: boolean = false;
   constructor(private cdr: ChangeDetectorRef,private _cookie:CookieService,private cartService: CartService,private _auth: AuthService) {
  
    }
  
    ngOnInit(): void {
      const storedData = this._auth.userData
      .subscribe({
        next: (response) => {
          this.userData = response;
          console.log(this.userData);
        },
        error: (error) => {
        }
      });
         
      if (this.userData) {
        this.token = this.userData.token;
        this.isLogin = true;
        this.cartService.getCartById().subscribe({
          next: (res) => {
            console.log('Cart data:', res);
            this.cartData = res;
          },
          error: (err) => console.error('Error loading cart:', err)
        });
      }
              
          
        

  }

  quantity: number = 1;

  increase() {
    this.quantity++;
  }

  decrease() {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  //get trader by id


  //remve item from cart
  RemoveItem(itemId:String):void {
    this.cartData.cartItems = this.cartData.cartItems.filter((item: any) => item.itemId !== itemId);
    console.log('Updated cart data:', this.cartData);
    this.cartService.removeItemFromCart(this.cartData).subscribe({
      next: (res) => {
        console.log('Item removed:', res);
        this.cartData = res;
        this.cdr.detectChanges();
        this.cartData = res;
        this.cartData.cartItems = [...this.cartData.cartItems];;
      },
      error: (err) => console.error('Error removing item:', err)
    });
   
  }
}

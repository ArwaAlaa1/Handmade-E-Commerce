import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-cart',
  imports: [],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  cartData: any = {};
 token: string = '';
 userData: any = null;
  imageBaseUrl: string = `https://handmadee-commerce.runasp.net/images//`;
  isLogin: boolean = false;
   constructor(private _cookie:CookieService,private cartService: CartService,private _auth: AuthService) {

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

}

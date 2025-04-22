import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart',
  imports: [],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  isLogin: boolean = false;
   constructor(private _AuthService : AuthService,private cartService: CartService) {
    this._AuthService.userData.subscribe({
      next: (data) => {
        if (data) {
          this.isLogin = true;
          var uaerdata=_AuthService.loadUserData();
          console.log(uaerdata);
          this.getCartById(data.userId);
          console.log(data.userId);
          console.log("succes");
        }
        else
        {
          this.isLogin = false;
          console.log("fail");
        }
      }
    });
    }
  
    ngOnInit(): void {
      console.log('CartComponent loaded');

      this._AuthService.userData.subscribe({
        next: (data) => {
          if (data) {
            this.isLogin = true;
            var uaerdata=this._AuthService.loadUserData();
            console.log(uaerdata);
            this.getCartById(data.userId);
            console.log(data.userId);
            console.log("succes");
          }
          else
          {
            this.isLogin = false;
            console.log("fail");
          }
        }
      });
    }
  getCartById(cartId: number) {
    this.cartService.getCartById(cartId).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }
  

}

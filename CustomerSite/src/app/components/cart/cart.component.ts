import { environment } from './../../../environments/environment.development';
import { ShippingService } from './../../services/shipping.service';
import { ChangeDetectorRef, Component, NgModule } from '@angular/core';
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
  shippingCosts: any[] = [];
  cartData: Cart= {} as Cart;
 token: string = '';
 userData: any = null;
  // imageBaseUrl: string = `https://handmadee-commerce.runasp.net/images//`;
  imageBaseUrl: string = `${environment.baseImageURL}images/`;
  isLogin: boolean = false;
  // deliveryCost: number = 0;
   constructor(private _shipCost:ShippingService,private cdr: ChangeDetectorRef,private _cookie:CookieService,private cartService: CartService,private _auth: AuthService) {

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
      }else{
        console.log('get cart from cookie');
      }


      this._shipCost.getShippingCosts().subscribe({
        next: (res) => {
          console.log('Shipping data:', res);
          this.shippingCosts = res;

        },
        error: (err) => console.error('Error loading Shipping Costs:', err)
      });
  }

  //get trader by id


  //remve item from cart
  RemoveItem(itemId:String):void {
    const index = this.cartData.cartItems.findIndex(item => item.itemId === itemId);
    this.cartData.cartItems.splice(index, 1);

    console.log('Updated cart data:', this.cartData);
    this.cartService.removeItemFromCart(this.cartData).subscribe({
      next: (res) => {
        console.log('Item removed:', res);
        // this.cartData = res;

        // this.cartData.cartItems = this.cartData.cartItems.filter((item: any) => item.itemId !== itemId);
        // this.cartService.getCartById().subscribe({
        //   next: (res) => {
        //     console.log('Cart data:', res);
        //     this.cartData = res;
        //   },
        //   error: (err) => console.error('Error loading cart:', err)
        // });


        // this.cartData.cartItems = [...this.cartData.cartItems];
      },
      error: (err) => console.error('Error removing item:', err)
    });

  }

//   GetDelivry(itemId:number): void {

//     console.log('Item ID:', itemId);
//     const selectedItem = this.shippingCosts.find(item => item.id == itemId);
//     console.log(selectedItem);

// }

deliveryCost: number = 0; // أو null حسب رغبتك

GetDelivry(event: Event): void {
  const selectElement = event.target as HTMLSelectElement;
  const selectedId = Number(selectElement.value);

  console.log('Selected ID:', selectedId);

  if (!selectedId || isNaN(selectedId)) {
    console.error('Invalid or empty value selected');
    this.deliveryCost = 0;
    return;
  }

  const selectedItem = this.shippingCosts.find(item => item.id === selectedId);

  if (!selectedItem) {
    console.error('Item not found');
    this.deliveryCost = 0;
    return;
  }

  console.log('Selected Item:', selectedItem);

  this.deliveryCost = selectedItem.cost;
}



}

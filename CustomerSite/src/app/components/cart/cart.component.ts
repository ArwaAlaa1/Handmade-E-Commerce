import { environment } from './../../../environments/environment.development';
import { ShippingService } from './../../services/shipping.service';
import { ChangeDetectorRef, Component, NgModule, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';
import { Cart } from '../../interfaces/cart';
import { UserService } from '../../services/user.service';
import { AddressPopUpComponent } from "../../address-pop-up/address-pop-up.component";
import { v4 as uuidv4 } from 'uuid';
import { Subscription } from 'rxjs';
declare var bootstrap: any;
@Component({
  selector: 'app-cart',
  imports: [CommonModule, AddressPopUpComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
// export class CartComponent {
  export class CartComponent implements OnInit, OnDestroy {
  shippingCosts: any[] = [];
  cartData: Cart= {} as Cart;
 token: string = '';
 userData: any = null;
 modal: any;
 addresses: any[] = [];
 addressSelected: any = null;
 selectedAddressIndex :number = 0;
 deliveryCost: number = 0;
 subTotal : number = 0;
  total: number = 0;
  subscriptions: Subscription[] = [];

  imageBaseUrl: string = `${environment.baseImageURL}images/`;
  isLogin: boolean = false;

   constructor(private _userService:UserService,private _shipCost:ShippingService,
    private cdr: ChangeDetectorRef,private _cookie:CookieService,
    private cartService: CartService,private _auth: AuthService) {

    }
    ngOnInit(): void {
      const userSub = this._auth.userData.subscribe({
        next: (response) => {
          this.userData = response;
          this.token = this.userData.token;
          this.isLogin = true;

          this.cartService.getCartById().subscribe({
            next: (res) => {
              this.cartData = res;
              if (this.cartData.addressId != null) {
                this.updateAddress(this.cartData.addressId);
              }
              this.calculateTotal(this.cartData);
            }
          });
        },
        error: (err) => console.error('Failed to get user data:', err)
      });
      this.subscriptions.push(userSub);

      const shippingSub = this._shipCost.getShippingCosts().subscribe({
        next: (res) => {
          this.shippingCosts = res;
        },
        error: (err) => console.error('Error loading Shipping Costs:', err)
      });
      this.subscriptions.push(shippingSub);
    }

    ngOnDestroy(): void {
      this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    calculateTotal(cartData: Cart): void {
      this.subTotal = 0;
      for (const item of cartData.cartItems) {
        if (item.priceAfterSale != 0) {
          this.subTotal += item.priceAfterSale;
        } else {
          this.subTotal += item.price;
        }
        // this.subTotal += item.priceAfterSale ?? item.price;
      }
      this.total = this.subTotal + this.deliveryCost;
    }

    updateAddress(addressId: number): void {
      if (addressId == null) {
        const sub = this._userService.getAllAddress().subscribe({
          next: (res) => {
            this.addressSelected = res[0];
            this.getDeliveryCost(this.addressSelected.city);
          },
          error: (err) => console.error('Error loading addresses:', err)
        });
        this.subscriptions.push(sub);
      } else {
        const sub = this._userService.getAddress(addressId).subscribe({
          next: (res) => {
            this.addressSelected = res;
            this.getDeliveryCost(this.addressSelected.city);
          },
          error: (err) => console.error('Error loading address:', err)
        });
        this.subscriptions.push(sub);
      }
    }

    getDeliveryCost(city: string): void {
      const selectedItem = this.shippingCosts.find(item => item.name === city);
      if (!selectedItem) {
        console.error('Shipping cost not found for city:', city);
        this.deliveryCost = 0;
        return;
      }
      this.deliveryCost = selectedItem.cost;
    }

    Increase(itemId: string): void {
      const item = this.cartData.cartItems.find(i => i.itemId === itemId);
      if (!item) return;

      item.quantity += 1;
      item.price = item.quantity * item.unitPrice;
      if (item.priceAfterSale != null) {
        item.priceAfterSale = item.quantity * item.priceAfterSale;
      }

      this.calculateTotal(this.cartData);

      this.cartService.updateCart(this.cartData).subscribe({
        next: (res) => {
          this.cartData = res;
          if (this.cartData.addressId != null) {
            this.updateAddress(this.cartData.addressId);
          }
          this.calculateTotal(this.cartData);
        },
        error: (err) => console.error('Error increasing item quantity:', err)
      });
    }


    Decrease(itemId: string): void {
      const item = this.cartData.cartItems.find(i => i.itemId === itemId);
      if (!item) return;

      if (item.quantity > 1) {
        item.quantity -= 1;
        item.price = item.quantity * item.unitPrice;
        if (item.priceAfterSale != null) {
          item.priceAfterSale = item.quantity * item.priceAfterSale;
        }

        this.calculateTotal(this.cartData);

        this.cartService.updateCart(this.cartData).subscribe({
          next: (res) => {
            this.cartData = res;
            if (this.cartData.addressId != null) {
              this.updateAddress(this.cartData.addressId);
            }
            this.calculateTotal(this.cartData);
          },
          error: (err) => console.error('Error decreasing item quantity:', err)
        });
      } else {
        console.warn('Minimum quantity is 1');
      }
    }



    RemoveItem(itemId: string): void {
      const index = this.cartData.cartItems.findIndex(item => item.itemId === itemId);
      if (index !== -1) {
        this.cartData.cartItems.splice(index, 1);
        this.updateCartAndTotals();
      }
    }


    updateCartAndTotals(): void {
      this.cartService.updateCart(this.cartData).subscribe({
        next: (res) => {
          this.cartData = res;
          if (this.cartData.addressId != null) {
            this.updateAddress(this.cartData.addressId);
          }
          this.calculateTotal(this.cartData);
        },
        error: (err) => console.error('Error updating cart:', err)
      });
    }





  openModal() {

      this._userService.getAllAddress().subscribe({
            next: (res) => {
              console.log('Address data:', res);
              this.addresses = res;
            },
            error: (err) => console.error('Error loading Address:', err)
          });


    const modalElement = document.getElementById('myModal');
    this.modal = new bootstrap.Modal(modalElement);
    this.modal.show();
  }

  closeModal() {
    if (this.modal) {
      this.modal.hide();
    }
  }

  onSelectedAddress(index: number) {
    this.selectedAddressIndex = index;
    this.addressSelected= this.addresses[index];

  this.cartData.addressId= this.addresses[index].id;
  this.cartService.updateCart(this.cartData).subscribe({
    next: (res) => {
      console.log('Update Delivry Address:', res);
      this.cartData = res;
      // this.getDeliveryCost(this.addresses[index].city);
      if(this.cartData.addressId != null) {
        this.updateAddress(this.cartData.addressId);
      }
     this.calculateTotal(this.cartData);

    },
    error: (err) => console.error('Error in Update Delivry Address:', err)
  });

  }

  Confirm():void{
    console.log('Selected Address Index from confirm:', this.selectedAddressIndex);
    this.closeModal();
  }
}

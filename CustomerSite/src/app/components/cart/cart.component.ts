import { environment } from './../../../environments/environment.development';
import { ShippingService } from './../../services/shipping.service';
import { ChangeDetectorRef, Component, NgModule } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';
import { Cart } from '../../interfaces/cart';
import { UserService } from '../../services/user.service';
import { AddressPopUpComponent } from "../../address-pop-up/address-pop-up.component";

declare var bootstrap: any; 
@Component({
  selector: 'app-cart',
  imports: [CommonModule, AddressPopUpComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
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

  imageBaseUrl: string = `${environment.baseImageURL}images/`;
  isLogin: boolean = false;

   constructor(private _userService:UserService,private _shipCost:ShippingService,
    private cdr: ChangeDetectorRef,private _cookie:CookieService,
    private cartService: CartService,private _auth: AuthService) {
  
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

      if (this.userData !=null) {
        this.token = this.userData.token;
        this.isLogin = true;
        this.cartService.getCartById().subscribe({
          next: (res) => {
            console.log('Cart data:', res);
            this.cartData = res;
            if (this.cartData.addressId != null) {
              this.addressSelected= this._userService.getAddress(this.cartData.addressId ).subscribe({
               next: (res) => {
                this.addressSelected = res;
                 console.log('Address data:', res);
                 this.getDeliveryCost(this.addressSelected.city);
          this.calculateTotal(this.cartData);
               },
               error: (err) => console.error('Error loading cart:', err)
               });
             }
            
            }
           
        });
      }else{
        console.log('get cart from cookie');
      }

    

      //Get shipping costs
      this._shipCost.getShippingCosts().subscribe({
        next: (res) => {
          console.log('Shipping data:', res);
          this.shippingCosts = res;

        },
        error: (err) => console.error('Error loading Shipping Costs:', err)
      });

     


  }

  //Calculate subtotal and total
  calculateTotal(cartdata:Cart): void {
    this.subTotal = 0;
    this.total = 0;
    for (const item of cartdata.cartItems) {
     
      if (item.sellingPrice != null) {
        this.subTotal += item.sellingPrice ;
      } else {
        this.subTotal += item.price ;
      }
    }
    this.total = this.subTotal + this.deliveryCost;
  }


  //remve item from cart
  RemoveItem(itemId:String):void {
    const index = this.cartData.cartItems.findIndex(item => item.itemId === itemId);
    this.cartData.cartItems.splice(index, 1);

    console.log('Updated cart data:', this.cartData);
    this.cartService.updateCart(this.cartData).subscribe({
      next: (res) => {
        console.log('Item removed:', res);
        // this.cartData = res;

        this.getDeliveryCost(this.addressSelected.city);
          this.calculateTotal(this.cartData);
     
        this.cartData.cartItems = [...this.cartData.cartItems];
      },
      error: (err) => console.error('Error removing item:', err)
    });
    
  }


//get delivery cost 
getDeliveryCost(city:string):void {
  const selectedItem = this.shippingCosts.find(item => item.name === city);
   
  if (!selectedItem) {
    console.error('Item not found');
    this.deliveryCost = 0;
    return ;
  }

  console.log('Selected Item:', selectedItem);

  this.deliveryCost = selectedItem.cost;
}
// Increase quantity
Increase(itemId:string): void {
  const itemIndex = this.cartData.cartItems.findIndex((cartItem: any) => cartItem.itemId === itemId);
  if (itemIndex !== -1) {
  
      this.cartData.cartItems[itemIndex].quantity += 1;
      if(this.cartData.cartItems[itemIndex].sellingPrice != null){
       
        var x= this.cartData.cartItems[itemIndex].quantity*this.cartData.cartItems[itemIndex].sellingPrice;
        this.cartData.cartItems[itemIndex].sellingPrice=x;
        
        this.cartData.cartItems[itemIndex].price
        = this.cartData.cartItems[itemIndex].quantity*this.cartData.cartItems[itemIndex].unitPrice;
    
      }
    else{
      this.cartData.cartItems[itemIndex].price
      = this.cartData.cartItems[itemIndex].quantity*this.cartData.cartItems[itemIndex].unitPrice;
  
    }
      this.cartService.updateCart(this.cartData).subscribe({
        next: (res) => {
          this.cartData = res;
        
          this.calculateTotal(this.cartData);
        },
        error: (err) => console.error('Error increasing item quantity:', err)
      });
    }
    
}
// Decrease quantity
Decrease(itemId: string): void {
  const itemIndex = this.cartData.cartItems.findIndex((cartItem: any) => cartItem.itemId === itemId);

  if (itemIndex !== -1) {
    const item = this.cartData.cartItems[itemIndex];

    if (item.quantity > 1) {
      item.quantity -= 1;
      if (item.sellingPrice != null) {
        item.priceAfterSale = item.quantity * item.sellingPrice;
      } else {
        item.priceAfterSale = item.price; // لو مافيش خصم، نخليه زي السعر الأصلي
      }
    } else {
     item.quantity=1
      console.warn('Minimum quantity reached.');
      return;
    }

    console.log('Updated cart data:', this.cartData);

    this.cartService.updateCart(this.cartData).subscribe({
      next: (res) => {
        console.log('Item quantity decreased:', res);
        this.cartData = res;
        this.calculateTotal(this.cartData);
      },
      error: (err) => console.error('Error decreasing item quantity:', err)
    });
  }
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
      this.getDeliveryCost(this.addresses[index].city);
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

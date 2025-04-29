
import { environment } from './../../../environments/environment.development';
import { ShippingService } from './../../services/shipping.service';
import { ChangeDetectorRef, Component, ElementRef, NgModule, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';
import { Cart } from '../../interfaces/cart';
import { UserService } from '../../services/user.service';
import { AddressPopUpComponent } from "../../address-pop-up/address-pop-up.component";
import { v4 as uuidv4 } from 'uuid';
import { PaymentService } from '../../services/payment.service';
import { Subscription } from 'rxjs';
import { loadStripe, Stripe, StripeElements, StripePaymentElement } from '@stripe/stripe-js'; 
import { log } from 'console';
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
  isLoading: boolean = false;
  
  errorMessage: string | null = null;
  stripe: Stripe | null = null;
  elements: StripeElements | null = null;
  paymentElement: StripePaymentElement | null = null;
  @ViewChild('paymentElement') paymentElementRef!: ElementRef; 
  clientSecret: string | null = null;
   constructor(private _userService:UserService,private _shipCost:ShippingService,
    private cdr: ChangeDetectorRef,private _cookie:CookieService,
    private cartService: CartService,private _auth: AuthService
  ,private PaymentService :PaymentService,private router: Router) {
  
}
async ngOnInit(): Promise<void> {
      this.stripe = await loadStripe('pk_test_51RHyXq2cgFmPnY2GeonmKkFjRwF7wksrIPULMfwthiHQ9qgD51r5sfH9J0UNdlzCgiT0tJkGTcJEdyhHKMIEYLLv00krvHhoDs');
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
    //

  
    ngOnDestroy(): void {
      this.subscriptions.forEach(sub => sub.unsubscribe());
    }
  
    // calculateTotal(cartData: Cart): void {
    //   this.subTotal = 0;
    //   for (const item of cartData.cartItems) {
    //     if (item.priceAfterSale != 0) {
    //       this.subTotal += item.priceAfterSale??0;
    //     } else {
    //       this.subTotal += item.price??0;
    //     }
    //     // this.subTotal += item.priceAfterSale ?? item.price;
    //   }
    //   this.total = this.subTotal + this.deliveryCost;
    // }
    calculateTotal(cartData: Cart): void {
      this.subTotal = 0;
      if (cartData.cartItems && cartData.cartItems.length > 0) {
        for (const item of cartData.cartItems) {
          const price = item.activeSale && item.priceAfterSale ? item.priceAfterSale : item.price;
          this.subTotal += (price ?? 0) * (item.quantity ?? 1);
        }
      }
      this.total = this.subTotal + (this.deliveryCost ?? 0);
      console.log('Subtotal:', this.subTotal, 'Delivery Cost:', this.deliveryCost, 'Total:', this.total);
      this.cdr.detectChanges();
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

  async onCheckout(): Promise<void> {
    if (!this.cartData.addressId) {
      this.errorMessage = 'Please select a delivery address before proceeding to checkout.';
      return;
    }

    if (!this.cartData.cartItems || this.cartData.cartItems.length === 0) {
      this.errorMessage = 'Your cart is empty. Add items to proceed to checkout.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    const shippingCostId = this.shippingCosts.find(item => item.name === this.addressSelected?.city)?.id;
    console.log('shippingCostId:', shippingCostId);
    const cartId= this.cartData.id;
    console.log('cartId:', cartId);
    

    
    this.PaymentService.createOrUpdatePayment(cartId, 35).subscribe({
      next: (response: Cart) => {
        this.isLoading = false;
        this.clientSecret = response.clientSecret?? null;

        // const backendTotal = response.cartItems.reduce((sum, item) => {
        //   return sum + ((item.priceAfterSale ?? item.price ?? 0) * item.quantity);
        // }, 0) + this.deliveryCost;

        // if (Math.abs(this.total - backendTotal) > 0.01) {
        //   this.errorMessage = 'Total mismatch detected. Please refresh the cart and try again.';
        //   return;
        // }
        if (!this.clientSecret || !this.stripe) {
          this.errorMessage = 'Failed to initialize payment. Please try again.';
          return;
        }

       
        this.elements = this.stripe.elements({ clientSecret: this.clientSecret });
        this.paymentElement = this.elements.create('payment');

        // this.paymentElement.mount(this.paymentElementRef.nativeElement);

        
        // this.paymentElement.on('ready', () => {
        //   console.log('Payment Element is ready');
        // });
        // setTimeout(() => {
        //   if (this.paymentElementRef && this.paymentElementRef.nativeElement) {
        //     this.paymentElement.mount(this.paymentElementRef.nativeElement);
  
        //     this.paymentElement.on('ready', () => {
        //       console.log('Payment Element is ready');
        //     });
        //   } else {
        //     console.error('Payment element reference not found in the DOM');
        //     this.errorMessage = 'Payment setup failed. Please try again.';
        //   }
        // }, 0);

        setTimeout(() => {
          if (
            this.paymentElement &&
            this.paymentElementRef &&
            this.paymentElementRef.nativeElement
          ) {
            this.paymentElement.mount(this.paymentElementRef.nativeElement);
        
            this.paymentElement.on('ready', () => {
              console.log('Payment Element is ready');
              console.log('Payment Element:', this.paymentElement);
              console.log(response.paymentId);
              console.log(this.clientSecret);
              
              
            });
          } else {
            console.error('Payment element reference not found in the DOM or paymentElement is null');
            this.errorMessage = 'Payment setup failed. Please try again.';
          }
        }, 0);
        
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Failed to initialize payment. Please try again.';
        console.error('Payment initialization error:', err);
      }
    });
  }


  async confirmPayment(): Promise<void> {
    if (!this.stripe || !this.elements || !this.clientSecret) {
      this.errorMessage = 'Payment setup is incomplete. Please try again.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    const { error, paymentIntent } = await this.stripe.confirmPayment({
      elements: this.elements,
      confirmParams: {
        return_url: `${window.location.origin}/order-confirmation` 
      },
      redirect: 'if_required' 
    });

    this.isLoading = false;

    if (error) {
      this.errorMessage = error.message || 'Payment failed. Please try again.';
      console.error('Payment error:', error);
    } else if (paymentIntent && paymentIntent.status === 'succeeded') {
      console.log('Payment succeeded:', paymentIntent);

     
    
      // Clear the cart in the backend
    this.cartService.clearCart(this.cartData.id).subscribe({
      next: () => {
        console.log('Cart cleared in the backend');
      },
      error: (err) => {
        console.error('Error clearing cart in the backend:', err);
        this.errorMessage = 'Payment succeeded, but failed to clear cart. Please contact support.';
      }
      
      });
      // Clear the cart in the frontend
    this.cartData = {} as Cart;
    this.cartData.cartItems = [];
    this.subTotal = 0;
    this.total = 0;
    this.deliveryCost = 0;

    this.router.navigate(['/order-confirmation'], {
      queryParams: { paymentId: paymentIntent.id }
    });
  }
  }
}
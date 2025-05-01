import { CartItem } from './../../interfaces/cart';
import { CartService } from './../../services/cart.service';
import { environment } from './../../../environments/environment.development';
import { ShippingService } from './../../services/shipping.service';
import { ChangeDetectorRef, Component, ElementRef, NgModule, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';
import { Cart } from '../../interfaces/cart';
import { UserService } from '../../services/user.service';
import { AddressPopUpComponent } from "../address-pop-up/address-pop-up.component";
import { v4 as uuidv4 } from 'uuid';
import { PaymentService } from '../../services/payment.service';
import { Subscription } from 'rxjs';
import { loadStripe, Stripe, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { OrderResponse } from '../../interfaces/order-response';
import { HttpErrorResponse } from '@angular/common/http';
 
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
    private cartService:CartService,private _auth: AuthService
  ,private PaymentService :PaymentService,private router: Router) {

}
async ngOnInit(): Promise<void> {
  this.stripe = await loadStripe('pk_test_51RHyXq2cgFmPnY2GeonmKkFjRwF7wksrIPULMfwthiHQ9qgD51r5sfH9J0UNdlzCgiT0tJkGTcJEdyhHKMIEYLLv00krvHhoDs');

  const userSub = this._auth.userData.subscribe({
    next: (response) => {
      this.userData = response;
      this.token = this.userData?.token;
      this.isLogin = true;

      // Authenticated: get user cart
      
      const existingCartId = this._cookie.get('cartId');

      if (this.token && existingCartId) {
        console.log('Guest cartId found in cookie:', existingCartId);
      
        // Step 1: Get logged-in user's cart
        this.cartService.getCartById().subscribe({
          next: (userCart) => {
            this.cartData = userCart;
      
            // Step 2: Get guest cart by cookie cartId
            console.log('Guest cartId found jjjjjin cookie:', existingCartId);
      
            this.cartService.getCarteById(existingCartId).subscribe({
              next: (guestCart) => {
                console.log('Guest cart items:', guestCart.cartItems);
      
                // Step 3: Merge guest cart items into user cart
                if (guestCart.cartItems?.length) {
                  this.cartData.cartItems = [...this.cartData.cartItems, ...guestCart.cartItems];
      
                  // Step 4: Update user's cart with merged items
                  this.cartService.updateCart(this.cartData).subscribe({
                    next: () => {
                      console.log('Merged guest cart items into user cart.');
      
                      // Step 5: Clear guest cart and delete cookie
                      this.cartService.clearCart(existingCartId).subscribe({
                        next: () => {
                          this._cookie.delete('cartId');
                          console.log('Guest cart cleared and cookie removed.');
                        },
                        error: (err) => console.error('Error clearing guest cart:', err)
                      });
                    },
                    error: (err) => console.error('Error updating user cart:', err)
                  });
                } else {
                  console.log('Guest cart is empty. Skipping merge.');
                  this._cookie.delete('cartId');
                }
      
                // Step 6: Load other user cart details
                if (this.cartData.addressId != null) {
                  this.updateAddress(this.cartData.addressId);
                }
                this.calculateTotal(this.cartData);
              },
              error: (err) => console.error('Error fetching guest cart:', err)
            });
          },
          error: (err) => console.error('Error fetching user cart:', err)
        });
            
    //   const existingCartId = this._cookie.get('cartId');
    //   if(existingCartId){
    //      // If cartId exists, fetch the cart and add the item to it
    //      console.log('Existing cartId:', existingCartId);
    // this.cartService.getCartById(existingCartId).subscribe({
    //   next: (responseCart) => {
    //     const cart = responseCart;
      
    //         this.cartService.getCartById(existingCartId).subscribe({
    //           next: (res) => {
    //             this.cartData = res;
    //             this._cookie.delete('cartId');
    //             console.log('Cart updated successfully:', res);
    //             this.cartService.clearCart(existingCartId).subscribe({
    //               next: () => {
    //                 console.log('Cart cleared successfully:', existingCartId);
                  
    //               },
    //               error: (err) => console.error('Error clearing cart:', err)
    //             });
    //             if (this.cartData.addressId != null) {
    //               this.updateAddress(this.cartData.addressId);
    //             }
    //             this.calculateTotal(this.cartData);

    //           },
    //           error: (err) => console.error('Failed to get cart:', err)
    //         });
    //       } ,
    //       error: (err) => console.error('Error updating cart:', err)
    //     });
    //   }else{
    //     this.cartService.getCartById().subscribe({
    //       next: (res) => {
    //         this.cartData = res;
    //         if (this.cartData.addressId != null) {
    //           this.updateAddress(this.cartData.addressId);
    //         }
    //         this.calculateTotal(this.cartData);
    //       },
    //       error: (err) => console.error('Failed to get cart:', err)
    //     });
    //   }
    
     
     }
     
    if (this.token) {
      this.cartService.getCartById().subscribe({
        next: (res) => {
          this.cartData = res;
        console.log('User cart data:', this.cartData);
          if (this.cartData.addressId != null) {
            this.updateAddress(this.cartData.addressId);
          }
          this.calculateTotal(this.cartData);
    
        },
        error: (err) => console.error('Failed to get cart:', err)
      });
    }
    },
    error: (err) => {
      console.error('Failed to get user data:', err);
    }
  });

  this.subscriptions.push(userSub);

  // Shipping cost
  const shippingSub = this._shipCost.getShippingCosts().subscribe({
    next: (res) => {
      this.shippingCosts = res;
    },
    error: (err) => console.error('Error loading Shipping Costs:', err)
  });

  this.subscriptions.push(shippingSub);

  // Guest: if no token, use cartId from cookie or create new one
  if (!this.token) {
    let cartId = this._cookie.get('cartId');

    if (cartId) {
      this.cartService.getCartById(cartId).subscribe({
        next: (res) => {
          this.cartData = res;
          console.log('Guest cart data:', this.cartData);
          this.calculateTotal(this.cartData);
        },
        error: (err) => {
          console.error('Failed to load guest cart:', err);
        }
      });
    }  
  }
}



    ngOnDestroy(): void {
      this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    calculateTotal(cartData: Cart): void {
      this.subTotal = 0;

      if(cartData==null)
    {
      this.subTotal = 0;
      this.total = this.subTotal + this.deliveryCost;
    }else{
      for (const item of cartData.cartItems) {
      
        item.price = item.quantity * item.unitPrice;

        if (item.activeSale && item.priceAfterSale > 0) {
          item.priceAfterSale = item.price - (item.price * item.activeSale/ 100);
          this.subTotal += item.priceAfterSale;
        } else {
          item.priceAfterSale = 0;
          this.subTotal += item.price;
        }
      }

      this.total = this.subTotal + this.deliveryCost;
    }
    }


    updateAddress(addressId: number): void {
      if (addressId == 0 && this.token != null) {
        const sub = this._userService.getAllAddress().subscribe({
          next: (res) => {
            this.addressSelected = res[0];
            this.getDeliveryCost(this.addressSelected.city);
          },
          error: (err) => console.error('Error loading addresses:', err)
        });
        this.subscriptions.push(sub);
      } else {
        if(this.token !=null){
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
      const item = this.cartData.cartItems?.find(i => i.itemId === itemId);
      if (!item) return;

      item.quantity += 1;

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
      const item = this.cartData.cartItems?.find(i => i.itemId === itemId);
      if (!item) return;

      if (item.quantity > 1) {
        item.quantity -= 1;

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
      const index = this.cartData.cartItems?.findIndex(item => item.itemId === itemId);
      if (index !== undefined && index !== -1) {
        this.cartData.cartItems.splice(index, 1);
        console.log('Item removed:', itemId);
        this.updateCartAndTotals();
      }
    }

    updateCartAndTotals(): void {

      console.log('Updating cart from null:', this.cartData);
      this.cartService.updateCart(this.cartData).subscribe({
        next: (res) => {
          if (res != null) {
            this.cartData = res;
            console.log('Updating cart from null:', this.cartData);

            if (!this.cartData.cartItems) {
              this.cartData.cartItems = [];
            }

            if (this.cartData.addressId && this.cartData.addressId !== 0) {
              this.updateAddress(this.cartData.addressId);
            }

            this.calculateTotal(this.cartData);
          } else {
           
            this.subTotal = 0;
            this.total = this.deliveryCost; // only delivery if any
          }

        },
        error: (err) => console.error('Error updating cart:', err)
      });

  }

  redirectToLogin(): void {
    this.router.navigate(['/login']);
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



    this.PaymentService.createOrUpdatePayment(cartId, shippingCostId).subscribe({
      next: (response: Cart) => {
        this.isLoading = false;
        this.cartData = response;
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

      const shippingCostId = this.shippingCosts.find(item => item.name === this.addressSelected?.city)?.id;
      console.log('shippingCostId:', shippingCostId);
      const cartId= this.cartData.id;
      console.log('cartId:', cartId);
      this.cartData.addressId =this.addressSelected.id;
      if(this.cartData.addressId == null){
        this.cartData.addressId=10
      }
     this.PaymentService.createOrder(cartId, shippingCostId, this.cartData.addressId,paymentIntent.id).subscribe({

      next: (response:OrderResponse) => {
        console.log('Order created in database:', response);

        // Clear the cart in the frontend
        this.cartData = {} as Cart;
        this.cartData.cartItems = [];
        this.subTotal = 0;
        this.total = 0;
        this.deliveryCost = 0;

        this.router.navigate(['/order-confirmation'], {
          queryParams: { paymentId: paymentIntent.id, orderId: response.orderId }
     
        });
      },
      error: (err : HttpErrorResponse) => {
        console.error('Error creating order in the backend:', err);
        this.errorMessage = 'Payment succeeded, but failed to create order. Please contact support.';
      }
    });
  }
}
    
  

  clearAllCart(): void {
    if (!this.cartData || !this.cartData.id) {
      console.warn('No cart data to clear');
      return;
    }
      // Clear the cart in the backend

    this.cartService.clearCart(this.cartData.id).subscribe({
      next: () => {
        console.log('Cart cleared in the backend');
  
        // Clear the cart in the frontend
        this.cartData = {} as Cart;
        this.cartData.cartItems = [];
        this.subTotal = 0;
        this.total = 0;
        this.deliveryCost = 0;
  
        // Update the UI
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error clearing cart in the backend:', err);
        this.errorMessage = 'Failed to clear cart. Please try again.';
      }
    });
  }
}
import { CommonService } from './../../services/common.service';
import { CartItem } from './../../interfaces/cart';
import { CartService } from './../../services/cart.service';
import { environment } from './../../../environments/environment.development';
import { ShippingService } from './../../services/shipping.service';
import { ChangeDetectorRef, Component, ElementRef, NgModule, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import Swal from 'sweetalert2';
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';
import { Cart } from '../../interfaces/cart';
import { UserService } from '../../services/user.service';
import { PaymentService } from '../../services/payment.service';
import { map, Observable, Subscription } from 'rxjs';
import { loadStripe, Stripe, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { OrderResponse } from '../../interfaces/order-response';
import { HttpErrorResponse } from '@angular/common/http';

 
declare var bootstrap: any;
// declare var toastr: any;
@Component({
  selector: 'app-cart',
  imports: [CommonModule ,RouterLink],
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
 extraCostTotal: number = 0;
 subTotal : number = 0;
  total: number = 0;
  subscriptions: Subscription[] = [];
  imageBaseUrl:string = environment.baseImageURL;
  // imageBaseUrl: string = `${environment.baseImageURL}images/`;


  isLogin: boolean = false;
  isLoading: boolean = false;

  errorMessage: string | null = null;
  stripe: Stripe | null = null;
  elements: StripeElements | null = null;
  paymentElement: StripePaymentElement | null = null;
  @ViewChild('paymentElement') paymentElementRef!: ElementRef;
  // @ViewChild('paymentElementRef', { static: false }) paymentElementRef!: ElementRef;
  clientSecret: string | null = null;
   constructor(private _userService:UserService,private _shipCost:ShippingService,
    private cdr: ChangeDetectorRef,private _cookie:CookieService,
    private cartService:CartService,private _auth: AuthService
  ,private PaymentService :PaymentService,private router: Router
, private commonService:CommonService) {
    
  }


async ngOnInit(): Promise<void> {
  this.stripe = await loadStripe('pk_test_51RHyXq2cgFmPnY2GeonmKkFjRwF7wksrIPULMfwthiHQ9qgD51r5sfH9J0UNdlzCgiT0tJkGTcJEdyhHKMIEYLLv00krvHhoDs');

  this.extraCostTotal = 0;
  const shippingSub = this._shipCost.getShippingCosts().subscribe({
    next: (res) => {
      this.shippingCosts = res;
    },
    error: (err) => console.error('Error loading Shipping Costs:', err)
  });
  this.subscriptions.push(shippingSub);


  const userSub = this._auth.userData.subscribe({
    next: (response) => {
      this.userData = response;
      this.token = this.userData?.token;
      this.isLogin = !!this.token;
      console.log('User data:', this.userData);
      console.log('Token:', this.token);

     
      const existingCartId = this._cookie.get('cartId');

      if (existingCartId || this.token) {
        const cartSub = this.cartService.getCartById(existingCartId).subscribe({
          next: (cart) => {
            this.cartData = cart || { id: existingCartId || '', cartItems: [] };
            console.log('Loaded cart data:', this.cartData);

            if (this.token && existingCartId) {
             
              const guestCartSub = this.cartService.getCarteById(existingCartId).subscribe({
                next: (guestCart) => {
                  console.log('Guest cart items:', guestCart.cartItems);

                  if (guestCart.cartItems?.length) {
                    this.cartData.cartItems = [...this.cartData.cartItems, ...guestCart.cartItems];

                    const updateSub = this.cartService.updateCart(this.cartData).subscribe({
                      next: () => {
                        console.log('Merged guest cart items into user cart.');

                        const clearSub = this.cartService.clearCart(existingCartId).subscribe({
                          next: () => {
                            this._cookie.delete('cartId');
                            console.log('Guest cart cleared and cookie removed.');
                          },
                          error: (err) => console.error('Error clearing guest cart:', err)
                        });
                        this.subscriptions.push(clearSub);
                      },
                      error: (err) => console.error('Error updating user cart:', err)
                    });
                    this.subscriptions.push(updateSub);
                  } else {
                    console.log('Guest cart is empty. Skipping merge.');
                    this._cookie.delete('cartId');
                  }
                },
                error: (err) => console.error('Error fetching guest cart:', err)
              });
              this.subscriptions.push(guestCartSub);
            }

            this.loadCartDetails();
          },
          error: (err) => console.error('Failed to get cart:', err)
        });
        this.subscriptions.push(cartSub);
      } else if (!this.token && !existingCartId) {
       
        this.cartData = { id: '', cartItems: [] };
        this.loadCartDetails();
      }
    },
    error: (err) => {
      console.error('Failed to get user data:', err);
    }
  });
  this.subscriptions.push(userSub);
}

private loadCartDetails(): void {
  console.log('Loading cart details with cartData:', this.cartData);
  if (this.cartData.addressId != null) {
    this.updateAddress(this.cartData.addressId);
  } 
  else 
  {
    
    const defaultAddressSub = this._userService.getAllAddress().subscribe({
      next: (addresses) => {
        if (addresses && addresses.length > 0) {
          this.addressSelected = addresses[0]; 
          this.cartData.addressId = this.addressSelected.id;
          this.getDeliveryCost(this.addressSelected.city);
          this.calculateTotal(this.cartData);

        
          const updateSub = this.cartService.updateCart(this.cartData).subscribe({
            next: (res) => {
              this.cartData = res;
              console.log('Cart updated with default address:', res);
              if (this.cartData.addressId != null && this.cartData.addressId !== 0) {
                this.updateAddress(this.cartData.addressId);
              } else {
               
                this.addressSelected = null;
                this.cartData.addressId = undefined;
                console.warn('Failed to save address in Redis, resetting address selection.');
              }
            },
            error: (err) => {
              console.error('Error updating cart with default address:', err);
              this.addressSelected = null;
              this.cartData.addressId = undefined;
            }
          });
          this.subscriptions.push(updateSub);
        }
        else {
          console.warn('No addresses found for user, address selection required.');
          this.addressSelected = null;
          this.cartData.addressId = undefined ;
        }
      },
      error: (err) => {
        console.error('Error loading default address:', err);
        this.addressSelected = null;
        this.cartData.addressId = undefined;
      }
    });
    this.subscriptions.push(defaultAddressSub);
  }
}
    ngOnDestroy(): void {
      this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    calculateTotal(cartData: Cart): void {
      this.subTotal = 0;
      this.extraCostTotal = 0;

      if (cartData == null || !cartData.cartItems || cartData.cartItems.length === 0) {
        this.subTotal = 0;
        this.extraCostTotal = 0; 
        this.total = this.deliveryCost;
      }else{
      for (const item of cartData.cartItems) {
      
        item.price = item.quantity * item.unitPrice;
        const itemExtraCostTotal = (item.extraCost || 0) * item.quantity;
        this.extraCostTotal += itemExtraCostTotal;
        console.log(`Item: ${item.productName}, Size: ${item.size}, ExtraCost: ${item.extraCost}, ItemExtraCostTotal: ${itemExtraCostTotal}`);
        if (item.activeSale && item.priceAfterSale > 0) {
          item.priceAfterSale = item.price - (item.price * item.activeSale/ 100);
          this.subTotal += item.priceAfterSale;
        } else {
          item.priceAfterSale = 0;
          this.subTotal += item.price;
        }
      }

      this.total = this.subTotal + this.deliveryCost + this.extraCostTotal;
      console.log(`SubTotal: ${this.subTotal}, ExtraCostTotal: ${this.extraCostTotal}, DeliveryCost: ${this.deliveryCost}, Total: ${this.total}`);
    }
    }

    updateAddress(addressId: number): void {
      if (!this.token || addressId === 0 || addressId == null) {
        this.addressSelected = null;
        this.getDeliveryCost(this.addressSelected?.city); 
        this.calculateTotal(this.cartData);
        return;
      }
    
      const subscription = this._userService.getAddress(addressId).subscribe({
        next: (res) => {
          this.addressSelected = res;
          this.getDeliveryCost(this.addressSelected.city);
          this.calculateTotal(this.cartData);
        },
        error: (err) => {
          console.error('Error loading address:', err);
          this.addressSelected = null;
          this.getDeliveryCost(this.addressSelected?.city);
          this.calculateTotal(this.cartData);
        }
      });
    
      subscription.add(() => this.subscriptions.push(subscription));
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
        },
        error: (err) => {
          console.error('Error increasing item quantity:', err);
          item.quantity -= 1;
          this.calculateTotal(this.cartData);
        }
      });
    }


    Decrease(itemId: string): void {
      const item = this.cartData.cartItems?.find(i => i.itemId === itemId);
      if (!item || item.quantity <= 1) return;
    
      const previousQuantity = item.quantity;
      item.quantity -= 1;
      this.calculateTotal(this.cartData);
    
      this.cartService.updateCart(this.cartData).subscribe({
        next: (res) => {
          this.cartData = res;
          if (this.cartData.addressId != null) {
            this.updateAddress(this.cartData.addressId);
          }
        },
        error: (err) => {
          console.error('Error decreasing item quantity:', err);
          item.quantity = previousQuantity; 
          this.calculateTotal(this.cartData);
        }
      });
    }

    RemoveItem(itemId: string): void {
      const index = this.cartData.cartItems?.findIndex(item => item.itemId === itemId);
      if (index === undefined || index === -1) return;
    
      this.cartData.cartItems.splice(index, 1);
      console.log('Item removed:', itemId);
      this.updateCartAndTotals();

      // if (!this.cartData.cartItems?.length || !this.addressSelected?.city) {
      //   this.getDeliveryCost(this.addressSelected?.city); 
      // }
     this.commonService.triggerRefresh();

      if (!this.cartData.cartItems?.length || !this.addressSelected?.city) {
        this.deliveryCost = 0;
        this.calculateTotal(this.cartData);
      }
    }

  updateCartAndTotals(): void {
    console.log('Updating cart from null:', this.cartData);
    this.cartService.updateCart(this.cartData).subscribe({
      next: (res) => {
        if (res) {
          this.cartData = res;
          if (!this.cartData.cartItems) {
            this.cartData.cartItems = [];
          }
          if (this.cartData.addressId && this.cartData.addressId !== 0) {
            this.updateAddress(this.cartData.addressId);
          }
        } else {
        
          this.cartData = {
            id: "0",
            cartItems: [],
            addressId: undefined, 
            paymentId: undefined, 
            clientSecret: undefined 
          };
        }
        this.calculateTotal(this.cartData);
        this.commonService.triggerRefresh();
      },
      error: (err) => console.error('Error updating cart:', err)
    });
  }

  redirectToLogin(): void {
    this.router.navigate(['/login']);
  }
  
  openModal(): void {
   
    const sub = this._userService.getAllAddress().subscribe({
      next: (res) => {
        console.log('Address data:', res);
        this.addresses = res;
      },
      error: (err) => console.error('Error loading Address:', err)
    });
    this.subscriptions.push(sub); 
  
    
    const modalElement = document.getElementById('myModal');
    if (modalElement) {
      this.modal = new bootstrap.Modal(modalElement);
      this.modal.show();
    } else {
      console.error('Modal element not found');
    }
  }

  closeModal() {
    if (this.modal) {
      this.modal.hide();
    }
  }

  onSelectedAddress(index: number): void {
    if (!this.addresses || index < 0 || index >= this.addresses.length) {
      console.error('Invalid address index:', index);
      return;
    }
  
    this.selectedAddressIndex = index;
    this.addressSelected = this.addresses[index];
    this.cartData.addressId = this.addresses[index].id;
  
    const sub = this.cartService.updateCart(this.cartData).subscribe({
      next: (res) => {
        this.cartData = res;
        console.log('Update Delivery Address:', res);
        if (this.cartData.addressId != null && this.cartData.addressId !== 0) {
          this.updateAddress(this.cartData.addressId);
        } else {
          this.addressSelected = null;
          this.cartData.addressId = undefined;
          console.warn('Failed to save address in Redis, resetting address selection.');
          alert('Failed to save the selected address. Please try again.');
        }
        this.calculateTotal(this.cartData);
      },
      error: (err) => {
        console.error('Error in Update Delivery Address:', err);
        this.addressSelected = null;
        this.cartData.addressId = undefined;
        this.calculateTotal(this.cartData);
      }
    });
    this.subscriptions.push(sub); 
  }

  Confirm(): void {
    console.log('Selected Address Index from confirm:', this.selectedAddressIndex);
    if (this.selectedAddressIndex >= 0 && this.addressSelected) {
      console.log('Confirmed address:', this.addressSelected);
    }
    this.closeModal();
  }

  async onCheckout(): Promise<void> {
    if (!this.cartData.addressId) {
      this.errorMessage = 'Please select a delivery address before proceeding to checkout.';
      await Swal.fire({
        icon: 'warning',
        title: 'Address Required',
        text: this.errorMessage??undefined,
        confirmButtonText: 'OK',
        confirmButtonColor: '#3085d6',
        customClass: {
          popup: 'animated fadeInDown'
        }
      });
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
        Swal.fire({
          icon: 'error',
          title: 'Payment Initialization Failed',
          text: this.errorMessage ?? undefined,
          confirmButtonText: 'OK',
          confirmButtonColor: '#dc3545',
          customClass: {
            popup: 'animated fadeInDown'
          }
        });
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

         Swal.fire({
          icon: 'success',
          title: 'Checkout Successful!',
          text: 'Your order has been placed successfully.',
          confirmButtonText: 'OK',
          confirmButtonColor: '#28a745'
        }).then((result) => {
          if (result.isConfirmed) {
            // Clear the cart in the frontend
            this.cartData = {} as Cart;
            this.cartData.cartItems = [];
            this.subTotal = 0;
            this.total = 0;
            this.deliveryCost = 0;
  
           
            this.router.navigate(['/home']); 
          }
        });
     
        // // Clear the cart in the frontend
        // this.cartData = {} as Cart;
        // this.cartData.cartItems = [];
        // this.subTotal = 0;
        // this.total = 0;
        // this.deliveryCost = 0;

     
        //   this.router.navigate(['/order-confirmation'], {
        //   queryParams: { paymentId: paymentIntent.id, orderId: response.orderId }
        //   });
      
      
      },
      error: (err : HttpErrorResponse) => {
        console.error('Error creating order in the backend:', err);
        this.errorMessage = 'Payment succeeded, but failed to create order. Please contact support.';
        
        Swal.fire({
          icon: 'error',
          title: 'failed to create order. Please contact support.',
          text: this.errorMessage ?? undefined, 
          confirmButtonText: 'Try Again',
          confirmButtonColor: '#dc3545'
        });
        }
    });
  }
}

async confirmPayment00(): Promise<void> {
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
    return;
  }

  if (paymentIntent && paymentIntent.status === 'succeeded') {
    console.log('Payment succeeded:', paymentIntent);

    const shippingCostId = this.shippingCosts.find(item => item.name === this.addressSelected?.city)?.id;
    const cartId = this.cartData.id;
    console.log('shippingCostId:', shippingCostId, 'cartId:', cartId);
    const addressIdToUse = this.cartData.addressId ?? this.addressSelected?.id;

    if (!addressIdToUse) {
      this.errorMessage = 'No valid delivery address selected.';
      return;
    }

    const sub = this.PaymentService.createOrder(cartId, shippingCostId, addressIdToUse, paymentIntent.id).subscribe({
      next: (response: OrderResponse) => {
        console.log('Order created in database:', response);

        // Clear the cart in the frontend
        this.cartData = {} as Cart;
        this.subTotal = 0;
        this.total = this.deliveryCost;
        this.deliveryCost = 0;

        this.router.navigate(['/order-confirmation'], {
          queryParams: { paymentId: paymentIntent.id, orderId: response.orderId }
        });
      },
      error: (err: HttpErrorResponse) => {
        console.error('Error creating order in the backend:', err);
        this.errorMessage = 'Payment succeeded, but failed to create order. Please contact support.';
      }
    });
    this.subscriptions.push(sub);
  }
}

trackByItemId(index: number, item: CartItem): string {
  return item.itemId; 
}
showPaymentTab: boolean = false;
proceedToCheckout(): void {
  const paymentTab = document.getElementById('payment-tab');
  if (paymentTab) {
    paymentTab.click(); 
    this.commonService.triggerRefresh();
  }
}
  clearAllCart(): void {
    if (!this.cartData || !this.cartData.id) {
      console.warn('No cart data to clear');
      return;
    }
  
    const sub = this.cartService.clearCart(this.cartData.id).subscribe({
      next: () => {
        console.log('Cart cleared in the backend');
  
        // Clear the cart in the frontend with a properly initialized Cart object
        this.cartData = {
          id: this.cartData.id, 
          cartItems: [],
          addressId: undefined,
          paymentId: undefined,
          clientSecret: undefined
        };
        this.subTotal = 0;
        this.total = 0;
        this.deliveryCost = 0;
        this.extraCostTotal = 0; // Reset extra cost total as well
  
        // Update the UI
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error clearing cart in the backend:', err);
        this.errorMessage = 'Failed to clear cart. Please try again.';
      }
    });
    this.subscriptions.push(sub); 
  }
}
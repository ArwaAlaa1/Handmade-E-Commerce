import { OrderService } from './order.service';
import { CartService } from './cart.service';
import { Injectable } from '@angular/core';
import { ProductService } from './product.service';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  constructor(private cartService:CartService,private orderService:OrderService
    ,private productService:ProductService) { }

    FavCount:number = 0;
    CartCount:number = 0;
    OrderCount:number = 0;
    private refreshNotifier = new BehaviorSubject<void>(undefined);
  refreshNotifier$ = this.refreshNotifier.asObservable();

  getCartCount() {
    this.cartService.getCartById().subscribe({
      next: (data) => {
        this.CartCount = data.cartItems.length;
        console.log("cartfiiir",data);
        console.log("cart countfirr",this.CartCount);
      }, error: (err) => {
        console.log(err);
      }
    });
  }

  getOrderCount() {
    this.orderService.getUserOrders().subscribe({
      next: (data) => {
        this.OrderCount = data.length;
        console.log("order count",this.OrderCount);
        console.log("order count",data);
      }, error: (err) => {
        console.log(err);
      }
    });
  }
// getFavCount() {
//       this.productService.GetFavList().subscribe({
//         next: (data) => {
//           this.FavCount = data.count;
//         }, error: (err) => {
//           console.log(err);
//         }
//       });
//     }
  triggerRefresh() {
    this.getCartCount();
    this.getOrderCount();
    this.refreshNotifier.next(); 
  }
}
    


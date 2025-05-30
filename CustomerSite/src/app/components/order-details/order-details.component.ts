import { Component } from '@angular/core';

import { OrderService } from '../../services/order.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-order-details',
  imports: [CommonModule,RouterLink],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css'
})
export class OrderDetailsComponent {

  constructor(private orderService: OrderService, private route: ActivatedRoute) { }
   imageBaseUrl: string = environment.baseImageURL;
  orderDetails:any = {};
  itemId:number = 0;
  orderId:number = 0;
    ngOnInit(): void {
      this.route.paramMap.subscribe(params => {
        const idParam = params.get('id');
        if (idParam) {
          this.orderId = +idParam; 
          console.log('Received Order ID:', this.orderId);
          this.orderService.getOrder(this.orderId).subscribe({
            next: (data) => {
              console.log("order",data);
              this.orderDetails = data;
            }, error: (err) => {
              console.log(err);
            }
          });
          // You can now call your API here
        }
      });
 
}
CancelItem(itemId:number){
  this.orderService.cancelOrderItem(itemId).subscribe({
    next: (data) => {
    //  this.orderDetails
      // this.orderDetails.orderItems = 
      const matchedItem = this.orderDetails.orderItems.find((item: { orderItemId: number; }) => item.orderItemId === itemId);
      console.log("matchedItem",matchedItem);
      matchedItem.itemStatus = "Cancelled";
      this.orderDetails.total -= matchedItem.totalPrice;
      this.orderDetails.subTotal -= matchedItem.totalPrice;
      console.log("cancel order item",data);
    }, error: (err) => {
      console.log(err);
    }
  });
}
get allItemsPending(): boolean {
  return this.orderDetails.orderItems.every((item: { itemStatus: string; }) => item.itemStatus === "Pending");
}
get allItemscancelled(): boolean {
  return this.orderDetails.orderItems.every((item: { itemStatus: string; }) => item.itemStatus === "Cancelled");
}
CancelOrder(id:number){
  this.orderService.cancelOrder(id).subscribe({
    next: (data) => {
      console.log("cancel order",this.orderDetails.id);
      console.log("cancel order",data);
      this.orderDetails = data;
    }, error: (err) => {
      console.log(err);
    }
  }); 
}
}
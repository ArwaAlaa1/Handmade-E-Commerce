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
          this.orderId = +idParam; // convert to number
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
      console.log("cancel item",itemId);
      console.log("cancel item",data);
      this.orderDetails = data;
    }, error: (err) => {
      console.log(err);
    }
  });
}
get allItemsPending(): boolean {
  return this.orderDetails.orderItems.every((item: { itemStatus: string; }) => item.itemStatus === "Pending");
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
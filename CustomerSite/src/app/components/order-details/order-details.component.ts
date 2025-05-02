import { Component } from '@angular/core';
import { SharedIdService } from '../../services/shared-id.service';
import { OrderService } from '../../services/order.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-order-details',
  imports: [CommonModule],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css'
})
export class OrderDetailsComponent {

  constructor(private sharedIdService:SharedIdService,private orderService:OrderService) { }
  orderDetails:any = {};
  itemId:number = 0;
  ngOnInit() {
    this.sharedIdService.orderId$.subscribe(id => {
      if (id !== null) {
       
        console.log("Received ID:", id);
        this.orderService.getOrder(id).subscribe({
          next: (data) => {
            console.log("order details",data);
            this.itemId=id;
            this.orderDetails = data;
           
          },
          error: (err) => {
            console.log(err);
          }
        });


      }
    });
 }
}
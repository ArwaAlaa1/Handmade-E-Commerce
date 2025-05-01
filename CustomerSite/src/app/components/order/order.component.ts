import { Component } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-order',
  imports: [CommonModule, FormsModule],
  templateUrl: './order.component.html',
  styleUrl: './order.component.css'
})
export class OrderComponent {

  constructor(private _orderServices:OrderService) { }
 orders:any[] = [];
  orderDetails:any = {};
  searchItem: string = '';
  ngOnInit(): void {
             this._orderServices.getUserOrders().subscribe({
               next: (data) => {
                 console.log("order",data);
                 this.orders = data;
               }, error: (err) => {
                 console.log(err);
               }
              });
             
  }

  get filteredOrders() {
    const term = this.searchItem.toLowerCase();
    return this.orders.filter(order =>
      order.status.toLowerCase().includes(term) ||
      (order.id + 1000).toString().includes(term)
    );
  }
  getOrder(orderid:number){
    this._orderServices.getOrder(orderid).subscribe({
      next: (data) => {
        console.log("Order Details",data);
      }, error: (err) => {
        console.log(err);
      }
     });
  }

}

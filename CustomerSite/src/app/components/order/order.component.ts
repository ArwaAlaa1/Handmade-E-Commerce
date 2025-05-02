import { provideServerRendering } from '@angular/platform-server';
import { Component } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SharedIdService } from '../../services/shared-id.service';

@Component({
  selector: 'app-order',
  imports: [CommonModule, FormsModule],
  templateUrl: './order.component.html',
  styleUrl: './order.component.css'
})
export class OrderComponent {

  constructor(private _orderServices:OrderService,private router:Router,private sharedIdService:SharedIdService) { }
 orders:any[] = [];
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

  viewOrderDetails(orderId: number) {
    this.sharedIdService.setOrderId(orderId);
    this.router.navigate(['/orderdetails']);
  }

}

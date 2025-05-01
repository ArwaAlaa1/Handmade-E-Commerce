// import { OrderService } from './../../services/order.service';
// import { CommonModule } from '@angular/common';
// import { Component, OnInit } from '@angular/core';
// import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';
// import { Order } from './../../interfaces/order';
// import { log } from 'node:console';

// @Component({
//   selector: 'app-order-confirmation',
//   imports: [CommonModule ,RouterLink,],
//   templateUrl: './order-confirmation.component.html',
//  styleUrl: './order-confirmation.component.css'
// })
// export class OrderConfirmationComponent implements OnInit {
//   orderId!: number;
//   paymentId!: string;
//   order!: Order;
//   isLoading = true;
//   error = '';

//   constructor(
//     private route: ActivatedRoute,
//     private orderService: OrderService
//   ) {}

//   ngOnInit(): void {
//     this.route.queryParams.subscribe(params => {
//       this.orderId = +params['orderId']; 
//       this.paymentId = params['paymentId'];
//       log('Order ID:', this.orderId);
//       log('Payment ID:', this.paymentId);
//       if (this.orderId) {
//         this.orderService.getOrderBtId(this.orderId).subscribe({
//           next: (res) => {
//             this.order = res;
//             this.isLoading = false;
//           },
//           error: (err) => {
//             this.error = 'An error occurred while loading the order.';
//             this.isLoading = false;
//           }
//         });
//       }
//     });
//   }

//   printInvoice() {
//     window.print();
//   }

//   get totalAmount() {
//     return this.order?.totalPrice ?? 0;
//   }
// }

import { OrderService } from './../../services/order.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-order-confirmation',
  imports: [CommonModule ,RouterLink,],
  templateUrl: './order-confirmation.component.html',
 styleUrl: './order-confirmation.component.css'
})
export class OrderConfirmationComponent implements OnInit {
  orderId!: string;
  paymentId!: string;

  constructor(private route: ActivatedRoute, private OrderService : OrderService ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.orderId = params['orderId'];
      this.paymentId = params['paymentId'];
  });
  }
  printInvoice() {
    window.print();
}
invoiceItems = [
  { productName: 'Handmade Bracelet', quantity: 2, unitPrice: 150, total: 300 },
  { productName: 'Leather Wallet', quantity: 1, unitPrice: 200, total: 200 }
];

get totalAmount() {
  return this.invoiceItems.reduce((sum, item) => sum + item.total, 0);
}
}
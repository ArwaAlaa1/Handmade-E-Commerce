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

  constructor(private route: ActivatedRoute ) {}

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
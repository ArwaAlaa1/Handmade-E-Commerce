import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-order-confirmation',
  template: `
    <div class="container mt-5">
      <h2>Order Confirmation</h2>
      <p>Thank you for your order!</p>
      <p>Payment ID: {{ paymentId }}</p>
    </div>
  `,
  styles: []
})
export class OrderConfirmationComponent implements OnInit {
  paymentId: string | null = null;

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.paymentId = this.route.snapshot.queryParamMap.get('paymentId');
  }
}
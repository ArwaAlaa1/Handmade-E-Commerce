import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedIdService {

  private orderIdSource = new BehaviorSubject<number | null>(null);
  orderId$ = this.orderIdSource.asObservable();

  setOrderId(id: number) {
    this.orderIdSource.next(id);
  }

}

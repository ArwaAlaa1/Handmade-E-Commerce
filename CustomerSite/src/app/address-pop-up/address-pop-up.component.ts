import { CommonModule } from '@angular/common';
import { UserService } from './../services/user.service';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-address-pop-up',
  imports: [CommonModule],
  templateUrl: './address-pop-up.component.html',
  styleUrl: './address-pop-up.component.css'
})
export class AddressPopUpComponent {
 
  selectedAddressIndex: number = 0;
  @Input() addresses: any[] = [];
 
   
  @Output() selectedAddress = new EventEmitter<number>();



  selectAddress(index: number) {
    this.selectedAddressIndex = index;
    this.selectedAddress.emit(this.selectedAddressIndex);
    // this.selectedAddress.emit(index);
  }
    modal: any;

 
  closeModal() {
    if (this.modal) {
      this.modal.hide();
    }
  }
  
    editAddress(index: number) {
      console.log('Edit address at index:', index);
      // Open edit form/modal
    }
  
    addNewAddress() {
      console.log('Add new address clicked');
      // Open add new address form/modal
    }
  
    // confirmAddress() {
    //   console.log('Confirmed address:', this.addresses[this.selectedIndex]);
    //   this.closeModal();
    // }
   
    
}

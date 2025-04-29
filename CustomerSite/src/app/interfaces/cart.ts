export interface Cart {
    id: string;
    paymentId?: string; 
    clientSecret?: string;
    cartItems: CartItem[];
    addressId: number;
  }

  export interface CartItem {
    itemId: string;
    productId: number;
    productName: string;
    sellerName: string;
    sellerId: string;
    photoUrl: string;
    category: string;
    customizeInfo?: string; 
    color?: string; 
    size?: string; 
    price: number; 
    unitPrice: number; 
    sellingPrice: number; 
    priceAfterSale: number; 
    activeSale: number; 
    quantity: number;
  }
  
  // export interface CartItem {
  //   itemId: string;
  //   productId: number;
  //   productName: string;
  //   sellerName: string;
  //   sellerId: string;
  //   photoUrl: string;
  //   category: string;
  //   customizeInfo: string;
  //   price: number;
  //   color: string;
  //   priceAfterSale: number;
  //   // priceAfterSale: number;
  //   unitPrice:number;
  //   size: string;
  //   activeSale: number;
  //   quantity: number;
  // }
  
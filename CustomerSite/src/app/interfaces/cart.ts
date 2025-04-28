export interface Cart {
    id: string;
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
    customizeInfo: string;
    price: number;
    color: string;
    sellingPrice: number;
    unitPrice:number;
    size: string;
    activeSale: number;
    quantity: number;
  }
  
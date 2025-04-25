export interface Cart {
    id: string;
    cartItems: CartItem[];
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

    size: string;
    activeSale: number;
    quantity: number;
  }
  
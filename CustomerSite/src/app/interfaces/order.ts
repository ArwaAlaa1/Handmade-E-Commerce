export interface Order {
    id: number;
    cartId: string;
    shippingCostId: number;
    addressId: number;
    paymentId: string;
    orderDate: Date;
    totalPrice: number;
    status: string; // e.g., "Pending", "Completed", etc.
    items: OrderItem[]; // List of items in the order
}

export interface OrderItem {
    id: number;
    orderId: number;
    productId: number;
    productName: string;
    sellerName: string;
    sellerId: string;
    photoUrl: string;
    category: string;
    customizeInfo?: string; // Optional field for customization info
    color?: string; // Optional field for color
    size?: string; // Optional field for size
    price: number; // Price of the item at the time of order
    unitPrice: number; // Unit price of the item
    sellingPrice: number; // Selling price of the item
    priceAfterSale: number; // Price after any applicable sale or discount
    activeSale: number; // Active sale ID or percentage
    quantity: number; // Quantity ordered
}


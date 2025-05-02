using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.Core.Repository.Contract;
using ECommerce.Core.Services.Contract;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepo;
        private readonly IPaymentService paymentService;

        public OrderService(ICartRepository cartRepository, IUnitOfWork unitOfWork, IOrderRepository orderRepo, IPaymentService paymentService)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _orderRepo = orderRepo;
            this.paymentService = paymentService;
        }
    
        public async Task<Order?> CreateOrderAsync(string CustomerEmail, string CartId, int shippingCostId, int ShippingAddressId, string paymentId)
        {
            // 1. Get Cart from cart repo
            var cart = await _cartRepository.GetCartAsync(CartId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return null; 
            }

            // 2. Create OrderItems from CartItems
            var OrderItems = new List<OrderItem>();

            foreach (var item in cart.CartItems)
            {
                
                Product? product = null;
                decimal finalPrice;

               
                if (item.Price <= 0 || item.Price == null)
                {
                 
                    product = await _unitOfWork.Repository<Product>()
                        .GetByIdWithIncludeAsync(item.ProductId, "Sales");

                    decimal sellingPrice = product.SellingPrice;
                    var currentSale = product.Sales?.FirstOrDefault(s =>
                        s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

                    finalPrice = currentSale != null
                        ? sellingPrice - (sellingPrice * currentSale.Percent / 100)
                        : sellingPrice;

                    item.Price = finalPrice; 
                }
                else
                {
                   
                    product = await _unitOfWork.Repository<Product>()
                        .GetByIdWithIncludeAsync(item.ProductId, "Sales");
                    finalPrice = item.Price.Value; 
                }

               
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {item.ProductId} not found.");
                }
                var extraCost = item.ExtraCost ?? 0;
                var totalItemPrice = (finalPrice + extraCost) * item.Quantity;
                product.Stock -= item.Quantity;
                var orderItem = new OrderItem(item.ProductId, item.CustomizeInfo, item.Color, item.Size, product.SellerId, item.Quantity)
                {
                    TotalPrice = totalItemPrice
                };

                OrderItems.Add(orderItem);
            }

            // 3. Calculate subtotal
            var subtotal = OrderItems.Sum(oi => oi.TotalPrice);

            // 4. Get shipping cost
            var shippingCost = await _unitOfWork.Repository<ShippingCost>().GetByIdAsync(shippingCostId);
            if (shippingCost == null)
            {
                throw new ArgumentNullException(nameof(shippingCost), "Shipping cost not found");
            }

            // 5. Calculate total price
            var totalPrice = subtotal ;

            // 6. Check for existing order with the same paymentId
            var existingOrder = await _orderRepo.GetOrderByPaymentIdAsync(paymentId);
            if (existingOrder != null)
            {
                await CancelOrder(existingOrder.Id);
            }

            // 7. Create the order
            var order = new Order(CustomerEmail, shippingCostId, ShippingAddressId, totalPrice, OrderItems, paymentId)
            {
                shippingCost = shippingCost,
            };

            // 8. Save to database
            await _unitOfWork.Repository<Order>().AddAsync(order);
            var rows = await _unitOfWork.SaveAsync();
            if (rows <= 0)
                return null;

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string Email)
        {
            var orders = await _orderRepo.GetUserOrdersAsync(Email);

            return (IReadOnlyList<Order>)orders;
        }

        public async Task<Order> GetOrderForUserAsync(int orderid)
        {
            var order = await _orderRepo.GetOrderForUserAsync(orderid);

            return order;
        }
        public async Task<Order> CancelOrder(int orderid)
        {
            var order = await _orderRepo.GetOrderForUserAsync(orderid);
            order.IsDeleted = true;
            order.Status = OrderStatus.Cancelled;
            foreach (var item in order.OrderItems)
            {
                item.Product.Seller = null;
                item.OrderItemStatus = ItemStatus.Cancelled;
            }
            _unitOfWork.Repository<Order>().Update(order);
             await _unitOfWork.SaveAsync();
           
            return order;
        }

        public async Task<OrderItem> CancelItemOrder(int orderItemId)
        {
            var orderitem = await _orderRepo.GetItemInOrderAsync(orderItemId);
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(orderitem.ProductId);
            product.Stock +=orderitem.Quantity;
            orderitem.IsDeleted = true;
            orderitem.OrderItemStatus = ItemStatus.Cancelled;
          
            await _unitOfWork.SaveAsync();

            return orderitem;

        }
        /*
      public async Task<Order?> CreateOrderAsync(string CustomerEmail, string CartId, int shippingCostId, int ShippingAddressId, string paymentId)
     {
         //1.Get Cart from cart repo
         var cart = await _cartRepository.GetCartAsync(CartId);

         //2 Get OrderItems  fro product 
         var OrderItems = new List<OrderItem>();

         if (cart?.CartItems?.Count() > 0)
         {
             foreach (var item in cart.CartItems)
             {
                 //ProductOrderDetails productDetails = new ProductOrderDetails();


                 if (item.Price <= 0)
                 {
                     var product = await _unitOfWork.Repository<Product>()
                     .GetByIdWithIncludeAsync(item.ProductId, "Sales");

                     decimal sellingPrice = product.SellingPrice;
                     var currentSale = product.Sales?.FirstOrDefault(s =>
                         s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

                     item.Price = currentSale != null
                         ? sellingPrice - (sellingPrice * currentSale.Percent / 100)
                         : sellingPrice;
                 }
                 var orderItem = new OrderItem(item.ProductId, item.CustomizeInfo, item.Color, item.Size, product.SellerId, item.Quantity)
                 {
                     TotalPrice = item.Price.Value* item.Quantity 
                 };

                 OrderItems.Add(orderItem);

             }
         }

         //calc subtotal

         var subtotal = OrderItems.Sum(OI => OI.TotalPrice);
         ////get shippingcost
         var ShippingCost = await _unitOfWork.Repository<ShippingCost>().GetByIdAsync(shippingCostId);
         if (ShippingCost == null)
         {
             throw new ArgumentNullException(nameof(ShippingCost), "Shipping cost not found");
         }
         var TotalPrice =subtotal + ShippingCost.Cost;
         //createorder
         var existingOrder = await _orderRepo.GetOrderByPaymentIdAsync(paymentId);
         if ( existingOrder != null)
         {
             await CancelOrder(existingOrder.Id);
             //await paymentService.CreateOrUpdatePaymentAsync(CartId, ShippingCost.Id); //**
         }

         var order = new Order(CustomerEmail, shippingCostId, ShippingAddressId, TotalPrice, OrderItems, paymentId)
         {
             shippingCost = ShippingCost,
         };
         //save to db
         await _unitOfWork.Repository<Order>().AddAsync(order);
         var rows = await _unitOfWork.SaveAsync();
         if (rows <= 0)
             return null;

         return order;


     }*/

    }
}

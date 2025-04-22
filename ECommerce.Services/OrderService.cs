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

        public OrderService(ICartRepository cartRepository, IUnitOfWork unitOfWork, IOrderRepository orderRepo)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _orderRepo = orderRepo;
        }
        public async Task<Order?> CreateOrderAsync(string CustomerEmail, string CartId, int shippingCostId, int ShippingAddressId)
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
                   
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                 
                    var orderitem = new OrderItem(item.ProductId,item.CustomizeInfo, item.Color,item.Size,product.SellerId,item.Quantity);
                    orderitem.TotalPrice = (decimal)(product.DiscountedPrice == 0 ? product.Cost * item.Quantity : product.DiscountedPrice * item.Quantity);

                    OrderItems.Add(orderitem);

                }
            }

            //calc subtotal

            var TotalPrice = OrderItems.Sum(OI => OI.TotalPrice);
            ////get shippingcost
            var ShippingCost = await _unitOfWork.Repository<ShippingCost>().GetByIdAsync(shippingCostId);
            TotalPrice += ShippingCost.Cost;
            //createorder
           
            var order = new Order(CustomerEmail, shippingCostId, ShippingAddressId, TotalPrice, OrderItems,"");
            //save to db
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
                item.OrderItemStatus = ItemStatus.Cancelled;
            }
            _unitOfWork.Repository<Order>().Update(order);
             await _unitOfWork.SaveAsync();
           
            return order;
        }

        public async Task<OrderItem> CancelItemOrder(int orderItemId)
        {
            var orderitem = await _orderRepo.GetItemInOrderAsync(orderItemId);

            orderitem.IsDeleted = true;
            orderitem.OrderItemStatus = ItemStatus.Cancelled;
          
            await _unitOfWork.SaveAsync();

            return orderitem;

        }

       
    }
}

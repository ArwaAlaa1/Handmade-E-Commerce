using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core;
using ECommerce.Core.Models.Cart;
using ECommerce.Core.Models.Order;
using ECommerce.Core.Repository.Contract;
using ECommerce.Core.Services.Contract;
using Microsoft.Extensions.Configuration;
using Stripe;


namespace ECommerce.Services
{
    public class PaymentService : IPaymentService
    {
        //private readonly IUnitOfWork unitOfWork;
        private readonly ICartRepository cartRepository;
        private readonly IConfiguration configuration;
        private readonly IProductRepository productRepository;
        private readonly IShippingCostRepository shippingCostRepository;


        public PaymentService(/*IUnitOfWork unitOfWork,*/ ICartRepository cartRepository, IProductRepository productRepository, IConfiguration configuration, IShippingCostRepository shippingCostRepository)
        {
            //this.unitOfWork = unitOfWork;
            this.cartRepository = cartRepository;
            this.productRepository = productRepository;
            this.configuration = configuration;
            this.shippingCostRepository = shippingCostRepository;
        }

        public async Task<Cart> CreateOrUpdatePaymentAsync(string CardId, int? shippingCostId)
        {
            //var cart = await unitOfWork.cartRepository.GetCartAsync(CardId);
            var cart = await cartRepository.GetCartAsync(CardId);
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart), "Cart not found");
            }

            StripeConfiguration.ApiKey = configuration["StripeSetting:secretKey"];
            var shippingPrice = 0m;
            /*
             if (shippingCostId != null)
            {
                var shippingCost = unitOfWork.ShippingCosts.GetShippingCostByIdAsync(shippingCostId.Value);
                if (shippingCost == null)
                {
                    throw new ArgumentNullException(nameof(shippingCost), "Shipping cost not found");
                }
                shippingPrice = shippingCost.Price;
            }*/
            if (shippingCostId.HasValue)
            {
                //var shippingCost = await unitOfWork.ShippingCosts.GetShippingCostByIdAsync(shippingCostId.Value);
                var shippingCost = await shippingCostRepository.GetShippingCostByIdAsync(shippingCostId.Value);
                if (shippingCost == null)
                {
                    throw new ArgumentNullException(nameof(shippingCost), "Shipping cost not found");
                }

                shippingPrice = shippingCost.Cost;

            }
            foreach (var item in cart.CartItems)
            {
                var product = await productRepository.GetProductByIDWithOffer(item.ProductId);
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product), "Product not found");
                }

                item.Price = product.SellingPrice;
            }
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent _intent;
            if (string.IsNullOrEmpty(cart.PaymentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)cart.CartItems.Sum(m => (m.Price*100) * m.Quantity) + (long)(shippingPrice * 100),
                    Currency = "USD",
                    PaymentMethodTypes = new List<string>{ "card" }
                };
                _intent = await paymentIntentService.CreateAsync(options);
                cart.PaymentId = _intent.Id;
                cart.ClientSecret = _intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)cart.CartItems.Sum(m => (m.Price * 100) * m.Quantity) + (long)(shippingPrice * 100),
                };
                _intent = await paymentIntentService.UpdateAsync(cart.PaymentId, options);
            }

            //await unitOfWork.cartRepository.UpdateBasketAsync(cart);
            await cartRepository.UpdateCartAsync(cart);
            return cart;
        }
    }
}

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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Issuing;


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
            try
            {
                //var cart = await unitOfWork.cartRepository.GetCartAsync(CardId);
                var cart = await cartRepository.GetCartAsync(CardId);
                if (cart == null)
                {
                    throw new ArgumentNullException(nameof(cart), "Cart not found");
                }

                StripeConfiguration.ApiKey = configuration["StripeSetting:secretKey"];
                var shippingPrice = 0m;

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

                    var currentSale = product.Sales?.FirstOrDefault(s =>
                                              s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

                    if (currentSale != null)
                    {
                        var discount = product.SellingPrice * currentSale.Percent / 100;
                        item.Price = product.SellingPrice - discount;
                    }
                    else
                    {
                        item.Price = product.SellingPrice;
                    }

                    item.ExtraCost??= 0;
                }
                PaymentIntentService paymentIntentService = new PaymentIntentService();
                PaymentIntent _intent;
                var itemsTotal = cart.CartItems.Sum(m => ((m.Price + (m.ExtraCost ?? 0)) * m.Quantity));
                var totalAmount = (long)((itemsTotal + shippingPrice) * 100);
                if (string.IsNullOrEmpty(cart.PaymentId))
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = totalAmount,
                        Currency = "USD",
                        PaymentMethodTypes = new List<string> { "card" }
                    };
                    _intent = await paymentIntentService.CreateAsync(options);
                    cart.PaymentId = _intent.Id;
                    cart.ClientSecret = _intent.ClientSecret;
                }
                // Check if there is an existing PaymentIntent
                if (!string.IsNullOrEmpty(cart.PaymentId))
                {
                    // Get the existing PaymentIntent to check its status
                    _intent = await paymentIntentService.GetAsync(cart.PaymentId);

                    // If the PaymentIntent has already succeeded, create a new one
                    if (_intent.Status == "succeeded")
                    {
                        var createOptions = new PaymentIntentCreateOptions
                        {
                            Amount = totalAmount,
                            Currency = "USD",
                            PaymentMethodTypes = new List<string> { "card" }
                        };
                        _intent = await paymentIntentService.CreateAsync(createOptions);
                        cart.PaymentId = _intent.Id;
                        cart.ClientSecret = _intent.ClientSecret;
                    }
                    else
                    {
                        // If the PaymentIntent is not succeeded, update it
                        var updateOptions = new PaymentIntentUpdateOptions
                        {
                            Amount = totalAmount
                        };
                        _intent = await paymentIntentService.UpdateAsync(cart.PaymentId, updateOptions);
                        cart.ClientSecret = _intent.ClientSecret;
                    }
                }
                else
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = totalAmount,
                        Currency = "USD",
                        PaymentMethodTypes = new List<string> { "card" }
                    };
                    _intent = await paymentIntentService.CreateAsync(options);
                    cart.PaymentId = _intent.Id;
                    cart.ClientSecret = _intent.ClientSecret;
                }

                //await unitOfWork.cartRepository.UpdateBasketAsync(cart);
                await cartRepository.UpdateCartAsync(cart);
                return cart;
            }
            catch (StripeException ex)
            {
                //return Results.Problem(detail: ex.Message, statusCode: 500);
                throw new ApplicationException("Payment failed: " + ex.Message);
            }
           
        }
    }
}

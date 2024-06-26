using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Collections.Generic;

namespace MvcMovie.Controllers
{
    public class StripeController : Controller
    {
        public IActionResult Index()
        {
            List<ProductEntity> productList = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Product = "The Garfield Movie Poster",
                    Rate = 1500,
                    Quantity = 2,
                    ImagePath = "img/Image1.jpg"
                },
                new ProductEntity
                {
                    Product = "The Garfield Movie Poster",
                    Rate = 1000,
                    Quantity = 3,
                    ImagePath = "img/Image2.jpeg"
                }
            };

            return View(productList);
        }

        public IActionResult CheckOut()
        {
            List<ProductEntity> productList = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Product = "The Garfield Movie Poster",
                    Rate = 1500,
                    Quantity = 2,
                    ImagePath = "img/Image1.jpg"
                },
                new ProductEntity
                {
                    Product = "The Garfield Movie Poster",
                    Rate = 1000,
                    Quantity = 3,
                    ImagePath = "img/Image2.jpeg"
                }
            };

            var domain = "https://localhost:7145/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"Stripe/OrderConfirmation",
                CancelUrl = domain + $"Stripe/Login",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "micnguyen2063@gmail.com"
            };

            foreach(var item in productList)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Rate * item.Quantity),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ToString(),
                        }
                    },
                    Quantity = item.Quantity
                };

                options.LineItems.Add(sessionListItem);
            }
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}

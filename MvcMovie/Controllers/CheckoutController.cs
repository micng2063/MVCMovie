using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using Stripe.Checkout;
using System;
using System.Collections.Generic;

namespace MvcMovie.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            List<Ticket> ticketList = new List<Ticket>
            {
                new Ticket
                {
                    TicketType = "Adult",
                    Price = 9.75,
                    Quantity = 1,
                    Description = ""
                },
                new Ticket
                {
                    TicketType = "Senior",
                    Price = 9.25,
                    Quantity = 1,
                    Description = "Age 60+"
                },
                new Ticket
                {
                    TicketType = "Child",
                    Price = 9.00,
                    Quantity = 1,
                    Description = "Age 1-11"
                }
            };

            return View(ticketList);
        }
        public IActionResult CheckOut()
        {
            List<Ticket> ticketList = new List<Ticket>
            {
                new Ticket
                {
                    TicketType = "Adult",
                    Price = 9.75,
                    Quantity = 1,
                    Description = ""
                },
                new Ticket
                {
                    TicketType = "Senior",
                    Price = 9.25,
                    Quantity = 1,
                    Description = "Age 60+"
                },
                new Ticket
                {
                    TicketType = "Child",
                    Price = 9.00,
                    Quantity = 1,
                    Description = "Age 1-11"
                }
            };

            var domain = "https://localhost:7145/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"Checkout/OrderConfirmation",
                CancelUrl = domain,
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "micnguyen2063@gmail.com"
            };

            foreach (var item in ticketList)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * item.Quantity * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.TicketType.ToString(),
                        }
                    },
                    Quantity = item.Quantity
                };

                options.LineItems.Add(sessionListItem);
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            TempData["Session"] = session.Id;

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation()
        {
            try
            {
                if (TempData["Session"] == null)
                {
                    throw new Exception("Session data not found.");
                }

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(TempData["Session"].ToString());

                string customMessage = "";

                if (session.PaymentStatus == "paid")
                {
                    customMessage = "You're all good to go!";
                }
                else
                {
                    customMessage = "Still working on this part.";
                }

                ViewBag.Message = customMessage;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
                return View();
            }
        }

    }
}

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

        [HttpPost]
        public IActionResult CheckOut(string[] TicketType, string[] Description, double[] Prices, int[] Quantities)
        {
            var ticketList = new List<Ticket>();

            for (int i = 0; i < TicketType.Length; i++)
            {
                ticketList.Add(new Ticket
                {
                    TicketType = TicketType[i],
                    Description = Description[i],
                    Price = Prices[i],
                    Quantity = Quantities[i]
                });
            }

            var domain = "https://localhost:7145/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "Checkout/OrderConfirmation",
                CancelUrl = domain,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "micnguyen2063@gmail.com"
            };

            foreach (var item in ticketList)
            {
                if (item.Quantity > 0)
                {
                    var sessionListItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.TicketType
                            }
                        },
                        Quantity = item.Quantity
                    };

                    options.LineItems.Add(sessionListItem);
                }
            }

            var service = new SessionService();
            Session session = service.Create(options);

            TempData["Session"] = session.Id;
            TempData["TicketList"] = Newtonsoft.Json.JsonConvert.SerializeObject(ticketList);

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation()
        {
            try
            {
                if (TempData["Session"] == null || TempData["TicketList"] == null)
                {
                    throw new Exception("Session data not found.");
                }

                var service = new SessionService();
                Session session = service.Get(TempData["Session"].ToString());

                string customMessage = session.PaymentStatus == "paid"
                    ? "You're all good to go!"
                    : "Still working on this part.";

                var ticketList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Ticket>>(TempData["TicketList"].ToString());

                ViewBag.Message = customMessage;
                ViewBag.Receipt = GenerateReceipt(ticketList);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
                return View();
            }
        }

        private string GenerateReceipt(List<Ticket> ticketList)
        {
            var totalAmount = 0.0;
            var receipt = $"Receipt - {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
            receipt += new string('_', 40) + "\n\n";

            foreach (var ticket in ticketList)
            {
                var itemTotal = ticket.Price * ticket.Quantity;
                receipt += $"{ticket.TicketType}: {ticket.Quantity} x ${ticket.Price:F2} = ${itemTotal:F2}\n";
                totalAmount += itemTotal;
            }

            receipt += new string('_', 40) + "\n";
            receipt += $"Total Amount: ${totalAmount:F2}\n";

            return receipt;
        }
    }
}

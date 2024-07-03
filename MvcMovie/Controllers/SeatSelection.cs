using Microsoft.AspNetCore.Mvc;

namespace MvcMovie.Controllers
{
    public class SeatSelection : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

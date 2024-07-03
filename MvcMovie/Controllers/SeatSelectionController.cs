using Microsoft.AspNetCore.Mvc;

namespace MvcMovie.Controllers
{
    public class SeatSelectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

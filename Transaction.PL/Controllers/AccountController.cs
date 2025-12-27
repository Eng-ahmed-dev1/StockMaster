using Microsoft.AspNetCore.Mvc;

namespace Transaction.PL.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

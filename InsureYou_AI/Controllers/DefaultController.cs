using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

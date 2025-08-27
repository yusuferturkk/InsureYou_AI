using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult SendChatWithAI()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultHeaderComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

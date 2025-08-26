using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultMobileMenuComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.AdminLayoutComponent
{
    public class _AdminBreadCrumbComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

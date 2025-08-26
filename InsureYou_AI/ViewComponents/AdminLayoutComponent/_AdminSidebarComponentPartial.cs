using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.AdminLayoutComponent
{
    public class _AdminSidebarComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

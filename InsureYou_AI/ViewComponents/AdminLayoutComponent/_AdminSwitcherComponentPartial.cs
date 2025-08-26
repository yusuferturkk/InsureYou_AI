using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.AdminLayoutComponent
{
    public class _AdminSwitcherComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

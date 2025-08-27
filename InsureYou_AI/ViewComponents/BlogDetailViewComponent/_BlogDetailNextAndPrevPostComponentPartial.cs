using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.BlogDetailViewComponent
{
    public class _BlogDetailNextAndPrevPostComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

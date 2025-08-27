using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.BlogViewComponent
{
    public class _BlogListTagCloudComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _BlogListTagCloudComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            //var tags = _context.Tags.ToList();
            return View();
        }
    }
}

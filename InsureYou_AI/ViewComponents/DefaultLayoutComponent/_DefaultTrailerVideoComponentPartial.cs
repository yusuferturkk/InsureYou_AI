using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultTrailerVideoComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultTrailerVideoComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.TrailerVideos.ToList();
            return View(values);
        }
    }
}

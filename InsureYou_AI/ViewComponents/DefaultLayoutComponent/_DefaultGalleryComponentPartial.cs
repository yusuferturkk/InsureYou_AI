using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultGalleryComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultGalleryComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Galleries.ToList();
            return View(values);
        }
    }
}

using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultSliderComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultSliderComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Sliders.ToList();
            return View(values);
        }
    }
}

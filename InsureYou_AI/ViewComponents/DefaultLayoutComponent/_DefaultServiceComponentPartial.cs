using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultServiceComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultServiceComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Services.ToList();
            return View(values);
        }
    }
}

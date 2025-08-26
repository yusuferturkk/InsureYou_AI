using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultFooterLast2ArticleComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultFooterLast2ArticleComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Articles.OrderByDescending(x => x.ArticleId).Skip(3).Take(2).ToList();
            return View(values);
        }
    }
}

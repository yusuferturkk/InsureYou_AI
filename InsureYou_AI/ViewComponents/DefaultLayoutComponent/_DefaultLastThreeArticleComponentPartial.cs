using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultLastThreeArticleComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultLastThreeArticleComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Articles.OrderByDescending(x => x.ArticleId).Include(z => z.AppUser).Include(y => y.Category).Take(3).ToList();
            return View(values);
        }
    }
}

using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace InsureYou_AI.ViewComponents.BlogViewComponent
{
    public class _BlogListLastThreeBlogComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _BlogListLastThreeBlogComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var blogs = _context.Articles.OrderByDescending(a => a.ArticleId).Take(3).ToList();
            return View(blogs);
        }
    }
}

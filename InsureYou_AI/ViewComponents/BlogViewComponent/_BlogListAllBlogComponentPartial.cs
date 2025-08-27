using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYou_AI.ViewComponents.BlogViewComponent
{
    public class _BlogListAllBlogComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _BlogListAllBlogComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var blogs = _context.Articles.Include(x => x.Category).Include(y => y.AppUser).ToList();
            return View(blogs);
        }
    }
}

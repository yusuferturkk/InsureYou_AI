using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYou_AI.ViewComponents.BlogDetailViewComponent
{
    public class _BlogDetailCommentListComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _BlogDetailCommentListComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int id)
        {
            ViewBag.articleId = id;
            var values = _context.Comments.Include(x => x.AppUser).Where(x => x.ArticleId == id && x.Status == "Yorum Onaylandı").ToList();
            return View(values);
        }
    }
}

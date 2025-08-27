using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.BlogDetailViewComponent
{
    public class _BlogDetailContentComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _BlogDetailContentComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int id)
        {
            ViewBag.articleId = id;
            var value = _context.Articles.Where(x => x.ArticleId == id).FirstOrDefault();
            return View(value);
        }
    }
}

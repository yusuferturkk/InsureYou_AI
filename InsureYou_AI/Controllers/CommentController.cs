using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYou_AI.Controllers
{
    public class CommentController : Controller
    {
        private readonly InsureContext _context;

        public CommentController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult CommentList()
        {
            var values = _context.Comments.Include(x => x.Article).Include(y => y.AppUser).ToList();
            return View(values);
        }
    }
}

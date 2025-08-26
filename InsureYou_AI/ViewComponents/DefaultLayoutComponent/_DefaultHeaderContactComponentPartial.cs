using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultHeaderContactComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultHeaderContactComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.email = _context.Contacts.Select(x => x.Email).FirstOrDefault();
            ViewBag.phone = _context.Contacts.Select(x => x.PhoneNumber).FirstOrDefault();
            return View();
        }
    }
}

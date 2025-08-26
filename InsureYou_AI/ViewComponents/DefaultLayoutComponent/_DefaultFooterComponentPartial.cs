using InsureYou_AI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.ViewComponents.DefaultLayoutComponent
{
    public class _DefaultFooterComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultFooterComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.description = _context.Contacts.Select(x => x.Description).FirstOrDefault();
            ViewBag.phone = _context.Contacts.Select(x => x.PhoneNumber).FirstOrDefault();
            ViewBag.email = _context.Contacts.Select(x => x.Email).FirstOrDefault();
            ViewBag.address = _context.Contacts.Select(x => x.Address).FirstOrDefault();
            return View();
        }
    }
}

﻿using InsureYou_AI.Context;
using InsureYou_AI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.Controllers
{
    public class MessageController : Controller
    {
        private readonly InsureContext _context;

        public MessageController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult MessageList()
        {
            var values = _context.Messages.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateMessage(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }

        public IActionResult RemoveMessage(int id)
        {
            var value = _context.Messages.Find(id);
            _context.Messages.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }

        [HttpGet]
        public IActionResult UpdateMessage(int id)
        {
            var value = _context.Messages.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }
    }
}

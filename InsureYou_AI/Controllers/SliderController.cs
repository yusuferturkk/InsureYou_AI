﻿using InsureYou_AI.Context;
using InsureYou_AI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.Controllers
{
    public class SliderController : Controller
    {
        private readonly InsureContext _context;

        public SliderController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult SliderList()
        {
            var values = _context.Sliders.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateSlider()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateSlider(Slider slider)
        {
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("SliderList");
        }

        public IActionResult RemoveSlider(int id)
        {
            var value = _context.Sliders.Find(id);
            _context.Sliders.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("SliderList");
        }

        [HttpGet]
        public IActionResult UpdateSlider(int id)
        {
            var value = _context.Sliders.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateSlider(Slider slider)
        {
            _context.Sliders.Update(slider);
            _context.SaveChanges();
            return RedirectToAction("SliderList");
        }
    }
}

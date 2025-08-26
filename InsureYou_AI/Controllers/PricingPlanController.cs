﻿using InsureYou_AI.Context;
using InsureYou_AI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.Controllers
{
    public class PricingPlanController : Controller
    {
        private readonly InsureContext _context;

        public PricingPlanController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult PricingPlanList()
        {
            var values = _context.PricingPlans.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreatePricingPlan()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePricingPlan(PricingPlan pricingPlan)
        {
            _context.PricingPlans.Add(pricingPlan);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }

        public IActionResult RemovePricingPlan(int id)
        {
            var value = _context.PricingPlans.Find(id);
            _context.PricingPlans.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }

        [HttpGet]
        public IActionResult UpdatePricingPlan(int id)
        {
            var value = _context.PricingPlans.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdatePricingPlan(PricingPlan pricingPlan)
        {
            _context.PricingPlans.Update(pricingPlan);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }
    }
}

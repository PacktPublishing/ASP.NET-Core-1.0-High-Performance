using System;
using Microsoft.AspNetCore.Mvc;

namespace FeatureSwitching.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var rand = new Random();
            if (rand.Next(99) < 25)
            {
                return View();
            }
            return View("IndexOld");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

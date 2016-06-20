using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ErrorLogging.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger Logger;
        public HomeController(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<HomeController>();
        }

        public IActionResult Index()
        {
            Logger.LogDebug("Home page loaded");
            return View();
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

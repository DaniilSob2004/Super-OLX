using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using OnlineClassifieds.Models;
using OnlineClassifieds.Services;

namespace OnlineClassifieds.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CurrentUserProvider _currentUserProvider;

        public HomeController(ILogger<HomeController> logger, CurrentUserProvider currentUserProvider)
        {
            _logger = logger;
            _currentUserProvider = currentUserProvider;
        }


        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
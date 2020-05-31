using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pets.UI.Models;
using Pets.UI.ReadServices;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pets.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Index([FromServices] ICatsReadService catsReadService)
        {
            _logger.LogInformation("viewing crazy cat owners"); //TODO Don't really need this, was just testing to see if it worked - would anyone ever use this normally?
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();
            return View(catOwnersViewModel);

            //TODO Decide what we do if there are no matches or an error etc.
        }

        public IActionResult Privacy()
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
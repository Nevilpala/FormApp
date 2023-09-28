using FormApp.BAL;
using FormApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;



namespace FormApp.Controllers
{
    [CheckAccess]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

        public IActionResult Index()
		{
            return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

         
        [Route("/Error")]
		[Route("/Error/{id?}")]
        public IActionResult Error(int statuscode)
        {
            return View();        
        }

	
	}
}
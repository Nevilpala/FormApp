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

	
	}
}
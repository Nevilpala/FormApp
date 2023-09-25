using Microsoft.AspNetCore.Mvc;

namespace FormApp.Controllers
{
	//[Route("")]
	public class ExtraController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}

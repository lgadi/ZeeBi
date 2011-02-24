using System.Web.Mvc;

namespace ZeeBi.UI.Controllers
{
	public class ApiController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		} 
		
	}
}
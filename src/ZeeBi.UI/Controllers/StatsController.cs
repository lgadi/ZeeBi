using System.Linq;
using System.Web.Mvc;
using ZeeBi.UI.DataAccess;

namespace ZeeBi.UI.Controllers
{
	public class StatsController : Controller
	{
		[HttpGet]
		public ActionResult Info(string id)
		{
			var url = DB.Urls.FindOneById(id);
			if (url == null)
				return Responses.NotFound;
			if (Request.AcceptTypes != null && Request.AcceptTypes.Contains("application/json"))
			{
				return Json(url, JsonRequestBehavior.AllowGet);
			}
			return View(url);
		}		
	}
}
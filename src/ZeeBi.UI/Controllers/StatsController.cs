using System.Linq;
using System.Web.Mvc;
using MongoDB.Driver.Builders;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.ViewModels.Stats;

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
			var pageViewCount = DB.PageViews.Count(Query.EQ("UrlId", id));
			return View(new StatsViewModel()
			            	{
			            		PageViewCount = pageViewCount,
			            		Url = url
			            	}	
						);
		}		
	}
}
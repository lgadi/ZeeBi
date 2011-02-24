using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
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

			var query = Query.EQ("UrlId", id);

			var pageViewCount = DB.PageViews.Count(query);

			return View(new StatsViewModel() {
				PageViewCount = pageViewCount,
				Url = url,
				PageViewsByUserAgent = GetPageViewsByUserAgent(query)				
			});
		}

		private Dictionary<string, int> GetPageViewsByUserAgent(IMongoQuery query)
		{
			var reduce = new BsonJavaScript("function(o, agg) { agg.count++; }");
			var results = DB.PageViews.Group(query, "UserAgent", new { count = 0 }.ToBsonDocument(), reduce, null).ToList();
			return results.ToDictionary(x => x["UserAgent"].AsString, x => x["count"].ToInt32());
		}
	}
}
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
			var map = new BsonJavaScript(@"
function() {
	emit(this.UserAgent, 1);
}");

			var reduce = new BsonJavaScript(@"
function(key, values) {
	var total = 0;
	for (var i in values) {
		total += values[i];
	}
	return total;
}");
			var results = DB.PageViews.MapReduce(query, map, reduce, MapReduceOptions.SetOutput(MapReduceOutput.Inline)).InlineResults.ToList();
			return results.ToDictionary(x => x["_id"].AsString, x => x["value"].ToInt32());
		}
	}
}
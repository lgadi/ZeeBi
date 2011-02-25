using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;
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
			GetPageViewsByDate(query);
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

		private Dictionary<string, int> GetPageViewsByDate(IMongoQuery query)
		{
			//TODO: Why is that not working???
			var map = new BsonJavaScript("map = function() {"+ 
											"day = Date.UTC(this.ViewedAt.getFullYear(), this.ViewedAt.getMonth(), this.ViewedAt.getDate());"+
											"emit({day: day, daynum: this.ViewedAt.getDate()}, {count: 1});"+
										 "}");

			var reduce = new BsonJavaScript("function(key, values) {" +
												"var count = 0;" +
												"values.forEach(function(v) {" +
													"count += v['count'];" +
												"}" +
												");" +
												"return {count: count};" +
			                                "}");
			var mrob = new MapReduceOptionsBuilder();
			mrob.SetOutput("pageviews_count");

			var results = DB.PageViews.MapReduce(query, map, reduce, mrob);
			var a = DB.Database.GetCollection("pageviews_count").FindAll();
			return null; //yeah, well - once it'll work figure out what to return.

		}
	}
}
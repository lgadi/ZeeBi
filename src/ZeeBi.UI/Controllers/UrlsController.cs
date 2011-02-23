using System;
using System.Web.Mvc;
using MongoDB.Driver;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.Controllers
{
    public class UrlsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Follow(string id)
		{
			var url = DB.Urls.FindOneById(id);
			if (url == null)
				return Responses.NotFound;

			RecordAnalytics(url);

			return new RedirectResult(url.LongUrl, false);
		}

    	private void RecordAnalytics(Url url)
    	{
//    		DB.Server
    	}

    	[HttpPost]
		public ActionResult Add(Url url)
		{
			try
			{
				AddUrl(url);
			}
			catch (IdAlreadyTakenException)
			{
				return new HttpStatusCodeResult(409, "ID already taken.");
			}
			return RedirectToAction("created", new { id = url.Id });
		}

    	private void AddUrl(Url url)
    	{
    		if (url.Id == null)
    		{
    			url.Id = GenerateFreeId();
    		}
    		else
    		{
				var existing = DB.Urls.FindOneById(url.Id);
				if (existing != null)
				{
					throw new IdAlreadyTakenException(existing.Id);
				}
    		}

    		DB.Urls.Insert(url, SafeMode.FSyncTrue);
    	}

    	private string GenerateFreeId()
    	{
    		string id;
    		do
    		{
    			id = GenerateId();
    		} while (DB.Urls.FindOneById(id) != null);
    		return id;
    	}

    	private string GenerateId()
    	{
    		return Guid.NewGuid().ToString().Substring(0, 6);
    	}

		[HttpGet]
		public ActionResult IsAvailable(string id)
		{
			var url = DB.Urls.FindOneById(id);
			if (url == null)
				return Json(true, JsonRequestBehavior.AllowGet);

			return Json(false, JsonRequestBehavior.AllowGet);
		}
		
		[HttpGet]
    	public ActionResult Created(string id)
    	{
			var url = DB.Urls.FindOneById(id);
			if (url == null)
				return Responses.NotFound; 
			
			return View(url);
    	}

    }

	public class IdAlreadyTakenException : Exception
	{
		public IdAlreadyTakenException(string id)
		{
			Id = id;
		}

		protected string Id { get; set; }
	}
}

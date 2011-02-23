using System;
using System.Web.Mvc;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.Controllers
{
    public class UIController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Follow(string id)
		{
			var url = DB.Urls.FindOneById(id);

			return new RedirectResult(url.LongUrl, false);
		}

		[HttpPost]
		public ActionResult Add(Url url)
		{
			AddUrl(url);
			return RedirectToAction("Created", new { id = url.Id });
		}

    	private void AddUrl(Url url)
    	{
    		if (url.Id == null)
    		{
    			url.Id = GenerateId();
    		}
    		else
    		{
				var existing = DB.Urls.FindOneById(url.Id);
				if (existing != null) throw new Exception("Already exists");
    		}

    		DB.Urls.Insert(url);
    	}

    	private string GenerateId()
    	{
    		return Guid.NewGuid().ToString().Substring(0, 6);
    	}

    	[HttpGet]
    	public ActionResult Created(string id)
    	{
    		throw new NotImplementedException();
    	}
    }
}

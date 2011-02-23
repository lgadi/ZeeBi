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
			if (url == null)
				return NotFoundResponse();

			return new RedirectResult(url.LongUrl, false);
		}

		[HttpPost]
		public ActionResult Add(Url url)
		{
			try
			{
				AddUrl(url);
			}
			catch (IdAlreadyTakenException ex)
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

    		DB.Urls.Insert(url);
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
    	public ActionResult Created(string id)
    	{
			var url = DB.Urls.FindOneById(id);
			if (url == null)
				return NotFoundResponse(); 
			
			return View(url);
    	}

    	private HttpStatusCodeResult NotFoundResponse()
    	{
    		return new HttpStatusCodeResult(404, "SORRY DUDE, NOT FOUND!");
    	}
    }

	internal class IdAlreadyTakenException : Exception
	{
		public IdAlreadyTakenException(string id)
		{
			Id = id;
		}

		protected string Id { get; set; }
	}
}

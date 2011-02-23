using System;
using System.Web.Mvc;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;
using ZeeBi.UI.Services;

namespace ZeeBi.UI.Controllers
{
    public class UrlsController : Controller
    {
    	private IdGenerator _idGenerator;

    	public UrlsController()
    	{
    		_idGenerator = new IdGenerator(); // todo IOC?
    	}

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Follow(string id)
		{
			var url = DB.Urls.FindOneById(id);
			if (url == null)
				return Responses.NotFound;

			return new RedirectResult(url.LongUrl, false);
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
    			url.Id = _idGenerator.Generate();
    		}
    		else
    		{
				if (_idGenerator.IsTaken(url.Id))
				{
					throw new IdAlreadyTakenException(url.Id);
				}
    		}

    		url.LongUrl = new UrlNormalizer().Normalize(url.LongUrl);
    		url.Created = DateTime.Now;

    		DB.Urls.Insert(url);
    	}

		[HttpGet]
		public ActionResult IsAvailable(string id)
		{
			var available = !_idGenerator.IsTaken(id);
			return Json(available, JsonRequestBehavior.AllowGet);
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

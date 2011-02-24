using System;
using System.Web.Mvc;
using ZeeBi.UI.Models;
using ZeeBi.UI.Services;

namespace ZeeBi.UI.Controllers
{
	public class UrlsController : Controller
	{
		private readonly UrlNormalizer _urlNormalizer;
		private readonly UrlsRepository _urlsRespository;

		public UrlsController()
		{
			// todo IOC?
			_urlNormalizer = new UrlNormalizer();
			_urlsRespository = new UrlsRepository();
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Follow(string id)
		{
			var url = _urlsRespository.FindOneById(id);
			if (url == null)
				return Responses.NotFound;
			Console.WriteLine(id);
			_urlsRespository.RecordAnalytics(url, Request);

			return new RedirectResult(url.LongUrl, false);
		}

	

		[HttpPost]
		public ActionResult Add(Url url)
		{
			try
			{
				_urlsRespository.AddUrl(url.LongUrl, url.Id);
			}
			catch (IdAlreadyTakenException)
			{
				return new HttpStatusCodeResult(409, "ID already taken.");
			}
			return RedirectToAction("created", new { id = url.Id });
		}

		

		[HttpGet]
		public ActionResult Created(string id)
		{
			var url = _urlsRespository.FindOneById(id);
			if (url == null)
				return Responses.NotFound;

			return View(url);
		}


		[HttpGet]
		public ActionResult IsAvailable(string id)
		{
			var available = _urlsRespository.IsAvailable(id);
			return Json(available, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult IsUrlValid(string url)
		{
			var valid = _urlNormalizer.IsValid(url);
			return Json(valid, JsonRequestBehavior.AllowGet);
		}
	}

	internal class InvalidUrlException : Exception
	{
		public string LongUrl { get; set; }

		public InvalidUrlException(string longUrl)
		{
			LongUrl = longUrl;
		}

		public override string Message
		{
			get { return "The specified URL is invalid."; }
		}
	}

	public class IdAlreadyTakenException : Exception
	{
		public IdAlreadyTakenException(string id)
		{
			Id = id;
		}

		protected string Id { get; set; }

		public override string Message
		{
			get { return "The specified ID is already taken."; }
		}
	}
}

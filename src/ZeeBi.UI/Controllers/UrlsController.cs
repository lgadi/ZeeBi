using System;
using System.Web.Mvc;
using MongoDB.Driver;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;
using ZeeBi.UI.Services;

namespace ZeeBi.UI.Controllers
{
	public class UrlsController : Controller
	{
		private readonly IdGenerator _idGenerator;
		private readonly UrlNormalizer _urlNormalizer;

		public UrlsController()
		{
			// todo IOC?
			_idGenerator = new IdGenerator(); 
			_urlNormalizer = new UrlNormalizer();
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

			RecordAnalytics(url);

			return new RedirectResult(url.LongUrl, false);
		}

		private void RecordAnalytics(Url url)
		{

			DB.PageViews.Insert(new PageView() {
				UrlId = url.Id,
				UserAgent = Request.UserAgent,
				UserIp = Request.UserHostAddress,
				ViewedAt = DateTime.Now
			});
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
			if (url.LongUrl == null) throw new InvalidUrlException(url.LongUrl);
			url.Created = DateTime.Now;

			DB.Urls.Insert(url, SafeMode.FSyncTrue);
		}

		[HttpGet]
		public ActionResult Created(string id)
		{
			var url = DB.Urls.FindOneById(id);
			if (url == null)
				return Responses.NotFound;

			return View(url);
		}


		[HttpGet]
		public ActionResult IsAvailable(string id)
		{
			var available = !_idGenerator.IsTaken(id);
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

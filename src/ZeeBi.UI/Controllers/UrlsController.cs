using System;
using System.Web.Mvc;
using MongoDB.Bson;
using ZeeBi.UI.Models;
using ZeeBi.UI.Services;
using ZeeBi.UI.ViewModels.Urls;

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
			RequestsLog.SaveRequestData(HttpContext);
			var model = new HomeViewModel();
			if (CurrentUser != null)
			{
				model.MyUrls = _urlsRespository.FindByUser(CurrentUser.Id);
			}

			model.LongUrl = TempData["Input.LongUrl"] as string;
			model.Id = TempData["Input.Id"] as string;
			model.TotalUrlsShortened = _urlsRespository.GetTotalUrlsShortened();
			return View(model);
		}

		protected User CurrentUser
		{
			get { return ViewData["currentUser"] as User; }
		}

		public ActionResult Follow(string id)
		{
			var url = _urlsRespository.FindById(id);
			if (url == null)
				return Responses.NotFound;
			
			_urlsRespository.RecordAnalytics(url, Request);

			return new RedirectResult(url.LongUrl, false);
		}

		[HttpPost]
		public ActionResult Add(string longUrl, string id)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(id)) id = null;

				var userId = CurrentUser == null ? ObjectId.Empty : CurrentUser.Id;
				var url = _urlsRespository.Add(longUrl, id, userId);
				return RedirectToAction("Created", new { id = url.Id });
			}
			catch (IdAlreadyTakenException)
			{
				return new HttpStatusCodeResult(409, "ID already taken.");
			}
			catch (InvalidUrlException)
			{
				TempData["Message"] = "This seems like an invalid URL. Wanna try again?";
				TempData["Input.LongUrl"] = longUrl;
				TempData["Input.Id"] = id;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		public ActionResult Created(string id)
		{
			var url = _urlsRespository.FindById(id);
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

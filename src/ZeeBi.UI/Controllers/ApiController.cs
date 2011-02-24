using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;
using ZeeBi.UI.Services;

namespace ZeeBi.UI.Controllers
{
	public class ApiController : Controller
	{
		private readonly UrlsRepository _urlsRespository;

		public ApiController()
		{
			_urlsRespository = new UrlsRepository();
		}

		[HttpGet]
		public ActionResult Index()
		{
			HttpCookie cookie = Request.Cookies.Get("userId");
			BsonObjectId userId = null;

			if (cookie != null)
			{
				var userIdString = cookie.Value;
				if (!string.IsNullOrEmpty(userIdString))
				{
					userId = new BsonObjectId(userIdString);
				}
			}
			if (userId != null)
			{
				ViewBag.UserId = userId.ToString();
			}
			return View();
		} 
		
		[HttpPost]
		public JsonResult Create(CreateRequest createRequest)
		{
			var target = _urlsRespository.AddUrl(createRequest.LongUrl);

			return Json(target);
		}

		[HttpGet]
		public ActionResult Get(string id)
		{
			var url = _urlsRespository.FindOneById(id);
			if (url == null)
			{
				return new HttpNotFoundResult("Could not find matching record for " + id);
			}
			return Json(url,JsonRequestBehavior.AllowGet);
		}

		public ActionResult SignOut()
		{
			this.Response.SetCookie(new HttpCookie("userId"));
			return RedirectToAction("index");
		}

		public ActionResult Authenticate(string token)
		{
			if (!string.IsNullOrEmpty(token))
			{
				var rpx = new Rpx("87df721ebccbde5919f4258b45d8f3d0dc1db546",
					"https://shcil.rpxnow.com/");
				var response = rpx.AuthInfo(token);

				var parser = new RpxResponseParser(response);

				if (parser.Status == RpxReponseStatus.Ok)
				{
					var responseUser = parser.BuildUser();
					User user = DB.Users.FindOne(Query.EQ("OpenId", responseUser.OpenId));
					if (user == null)
					{
						user = new User
						      {
						                		Email = responseUser.Email,
												Friendly = responseUser.Friendly,
												OpenId = responseUser.OpenId,
												UserName = responseUser.UserName
						                	 		
						       	};
						DB.Users.Insert(user);
					}

					Response.AppendCookie(new HttpCookie("userId", user.Id.ToString()));
					
					return RedirectToAction("index");
				}
			}

			ViewBag.Message = "There was a problem signing you in."
				+ "Verify your credentials and try again.";

			return RedirectToAction("index");
		}

		public class CreateRequest
		{
			public string LongUrl { get; set; }
		}
	}
}
using System.Web.Mvc;
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
			return View();
		} 
		
		[HttpPost]
		public JsonResult Create(CreateRequest createRequest)
		{
			var target = _urlsRespository.Add(createRequest.LongUrl);

			return Json(target);
		}

		[HttpGet]
		public ActionResult Get(string id)
		{
			var url = _urlsRespository.FindById(id);
			if (url == null)
			{
				return new HttpNotFoundResult("Could not find matching record for " + id);
			}
			return Json(url,JsonRequestBehavior.AllowGet);
		}

		public class CreateRequest
		{
			public string LongUrl { get; set; }
		}
	}
}
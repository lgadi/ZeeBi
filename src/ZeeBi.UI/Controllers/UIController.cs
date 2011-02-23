using System.Web.Mvc;
using ZeeBi.UI.DataAccess;

namespace ZeeBi.UI.Controllers
{
    public class UIController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Redirect(string id)
		{
			var url = DB.Urls.FindOneById(id);
			if (url == null)
				return new HttpStatusCodeResult(404, "SORRY DUDE, NOT FOUND!");

			return new RedirectResult(url.LongUrl, false);
		}
    }
}

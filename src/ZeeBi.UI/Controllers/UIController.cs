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

			return new RedirectResult(url.LongUrl, false);
		}
    }
}

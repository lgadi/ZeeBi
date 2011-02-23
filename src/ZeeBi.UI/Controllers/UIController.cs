using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
			return new RedirectResult("http://www.google.com?q=" + id, false);
		}
    }
}

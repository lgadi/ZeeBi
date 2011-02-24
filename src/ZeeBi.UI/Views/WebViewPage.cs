using System.Web;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.Views
{
	public abstract class WebViewPage<T> : System.Web.Mvc.WebViewPage<T>
	{
		public User CurrentUser
		{
			get { return ViewData["currentUser"] as User; }
		}

		public static string FullRoot
		{
			get { return "http://" + HttpContext.Current.Request.Url.Authority; }
		}
	}
}

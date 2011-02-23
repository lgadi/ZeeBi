using System.Web;
using System.Web.Mvc;

namespace ZeeBi.UI.Views
{
	public abstract class WebViewPage<T> : System.Web.Mvc.WebViewPage<T>
	{
		public static string FullRoot
		{
			get { return "http://" + HttpContext.Current.Request.Url.Authority; }
		}
	}
}
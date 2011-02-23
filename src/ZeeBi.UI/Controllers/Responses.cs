using System.Web.Mvc;

namespace ZeeBi.UI.Controllers
{
	public static class Responses
	{
		public static readonly HttpStatusCodeResult NotFound = new HttpStatusCodeResult(404, "SORRY DUDE, NOT FOUND!");
	}
}
using System.Web.Mvc;
using ZeeBi.UI.Services;

namespace ZeeBi.UI.Filters
{
	public class AuthenticationFilter : IActionFilter
	{
		private UsersRepository _usersRepository;

		public AuthenticationFilter()
		{
			_usersRepository = new UsersRepository();
		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var cookie = filterContext.HttpContext.Request.Cookies["userId"];
			if (cookie == null) return;
			var userId = cookie.Value;
			if (string.IsNullOrEmpty(userId)) return;

			var user = _usersRepository.FindById(userId);
			if (user == null) return;

			filterContext.Controller.ViewData["currentUser"] = user;
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
		}
	}
}
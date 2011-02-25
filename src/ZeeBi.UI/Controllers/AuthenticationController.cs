using System;
using System.Web;
using System.Web.Mvc;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;
using ZeeBi.UI.Services;

namespace ZeeBi.UI.Controllers
{
	public class AuthenticationController : Controller
	{
		private readonly UsersRepository _usersRepository;

		public AuthenticationController()
		{
			_usersRepository = new UsersRepository();
		}

		private RedirectResult ReturnToUrl(string returnUrl)
		{
			return Redirect(returnUrl ?? "/");
		}

		public ActionResult SignOut(string returnUrl)
		{
			var cookie = new HttpCookie("userId");
			cookie.Expires = new DateTime(1970, 1, 1);
			Response.SetCookie(cookie);

			return ReturnToUrl(returnUrl);
		}

		public ActionResult Authenticate(string token, string returnUrl)
		{
			if (string.IsNullOrEmpty(token))
			{
				return ReturnWithError(returnUrl);
			}
			
			var rpx = new Rpx("87df721ebccbde5919f4258b45d8f3d0dc1db546", "https://shcil.rpxnow.com/");
			var response = rpx.AuthInfo(token);
			var parser = new RpxResponseParser(response);
			if (parser.Status != RpxReponseStatus.Ok)
			{
				return ReturnWithError(returnUrl);
			}
			else
			{
				var responseUser = parser.BuildUser();
				var user = _usersRepository.FindByOpenId(responseUser.OpenId);
				if (user == null)
				{
					user = new User {
						Email = responseUser.Email,
						Friendly = responseUser.Friendly,
						OpenId = responseUser.OpenId,
						UserName = responseUser.UserName
					};
					DB.Users.Insert(user);
				}

				Response.AppendCookie(new HttpCookie("userId", user.Id.ToString()));

				return ReturnToUrl(returnUrl);
			}
		}

		private ActionResult ReturnWithError(string returnUrl)
		{
			TempData["Message"] = "There was a problem signing you in. Verify your credentials and try again.";

			return ReturnToUrl(returnUrl);
		}
	}
}
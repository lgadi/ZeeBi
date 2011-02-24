using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.Controllers
{
	public class InfoController : Controller
	{
		public ActionResult Version()
		{
			var version = typeof(Url).Assembly.GetName().Version.ToString(4);

			var text = new StringBuilder();
			text.AppendLine("version: " + version);
			try
			{
				text.AppendLine(System.IO.File.ReadAllText(Server.MapPath("~/buildinfo.txt")));
			}
			catch (Exception)
			{
			}

			return Content(text.ToString(), "text/plain");
		}
	}
};
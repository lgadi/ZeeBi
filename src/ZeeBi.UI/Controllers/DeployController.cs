using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ZeeBi.UI.Controllers
{
	public class DeployController : Controller
	{
		[HttpPost]
		public ActionResult Deploy()
		{
			var sourcesUrl = "https://github.com/lgadi/ZeeBi/zipball/master";

			var t = Task.Factory.StartNew(() =>
			{
				var client = new WebClient();
				client.DownloadDataCompleted += (s, e) =>
				{
					var sourcesDir = @"C:\ZeeBi\sources";
					var sourcesZip = @"C:\ZeeBi\sources";
					System.IO.File.WriteAllBytes(sourcesZip, e.Result);

					if (Directory.Exists(sourcesDir)) Directory.Delete(sourcesDir, true);

					var zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
					zip.ExtractZip(sourcesZip, sourcesDir, string.Empty);

					var startInfo = new ProcessStartInfo("deploy.bat")
					{
						CreateNoWindow = true,
						WorkingDirectory = sourcesDir,
					};
					var p = Process.Start(startInfo);
					p.WaitForExit();
				};

				client.DownloadDataAsync(new Uri(sourcesUrl));
			});

			return Content("OK");
		}
	}
}
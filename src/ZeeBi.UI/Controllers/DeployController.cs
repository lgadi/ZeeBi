﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZeeBi.UI.Controllers
{
	public class DeployController : Controller
	{
		public ActionResult Deploy(string payload)
		{

			var sentPayload = JsonConvert.DeserializeObject<JObject>(payload);

			var head = GetHead(sentPayload);
			var commits = GetCommits(sentPayload);

			var sourcesUrl = "https://github.com/lgadi/ZeeBi/zipball/master";
			var logFile = @"C:\ZeeBi\deploy.log";

			System.IO.File.AppendAllText(logFile, "\r\nDeploying starts at " + DateTime.UtcNow);

			var t = Task.Factory.StartNew(() =>
			{
				var client = new WebClient();
				client.DownloadDataCompleted += (s, e) =>
				{
					System.IO.File.AppendAllText(logFile, "sources arrived\r\n");
					var output = new StringBuilder();
					try
					{
						var sourcesDir = @"C:\ZeeBi\sources";
						var sourcesZip = @"C:\ZeeBi\sources.zip";
						System.IO.File.WriteAllBytes(sourcesZip, e.Result);

						if (Directory.Exists(sourcesDir)) Directory.Delete(sourcesDir, true);

						var zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
						zip.ExtractZip(sourcesZip, sourcesDir, string.Empty);
						System.IO.File.AppendAllText(logFile, "sources unzipped\r\n");

						// the data is actually in a sub-folder:
						sourcesDir = Directory.GetDirectories(sourcesDir)[0];

						var startInfo = new ProcessStartInfo(Path.Combine(sourcesDir,"deploy.bat"))
						{
							CreateNoWindow = true,
							WorkingDirectory = sourcesDir,
							RedirectStandardOutput = true,
							RedirectStandardError = true,
							UseShellExecute = false,
							ErrorDialog = false,
						};
						startInfo.EnvironmentVariables.Add("GIT_COMMIT_HEAD", head);
						startInfo.EnvironmentVariables.Add("GIT_COMMITS", commits);
						var p = Process.Start(startInfo);
						p.OutputDataReceived += (proc, data) => output.AppendLine(data.Data);
						p.BeginOutputReadLine();
						p.ErrorDataReceived += (proc, data) => output.AppendLine(data.Data);
						p.BeginErrorReadLine();
						p.WaitForExit();
					}
					catch (Exception ex)
					{
						System.IO.File.AppendAllText(logFile, "\r\n");
						System.IO.File.AppendAllText(logFile, "EXCEPTION:\r\n");
						System.IO.File.AppendAllText(logFile, ex.ToString());
						System.IO.File.AppendAllText(logFile, "\r\n");
					}
					finally
					{
						System.IO.File.AppendAllText(logFile, "deploy output:\r\n");
						System.IO.File.AppendAllText(logFile, output + "\r\n");
						System.IO.File.AppendAllText(logFile, "==============\r\n");
					}
				};

				client.DownloadDataAsync(new Uri(sourcesUrl));
			});
			System.IO.File.AppendAllText(logFile, "\r\nDownloading sources ... ");

			return Content("OK");
		}

		private string GetHead(JObject sentPayload)
		{
			try
			{
				return sentPayload["after"].ToString();
			}
			catch (Exception)
			{
				return "unknown";
			}
		}

		private string GetCommits(JObject payload)
		{
			try
			{
				var commits =  payload["commits"]
					.Cast<JObject>()
					.OrderByDescending(c=>c["timestamp"].ToString())
					.ToArray();

				return JsonConvert.SerializeObject(commits, Formatting.Indented);
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}
}
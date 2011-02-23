using System;

namespace ZeeBi.UI.Services
{
	public class UrlNormalizer
	{
		public bool IsValid(string url)
		{
			return ParseUrl(url) != null;
		}

		public string Normalize(string url)
		{
			var uri = ParseUrl(url);
			return uri == null ? null : uri.ToString();
		}

		private Uri ParseUrl(string url)
		{
			var parts = url.Split(new[] { Uri.SchemeDelimiter }, StringSplitOptions.None);
			if (parts.Length < 2) url = "http://" + url;

			Uri uri;
			var valid = Uri.TryCreate(url, UriKind.Absolute, out uri);
			return valid ? uri : null;
		}
	}
}
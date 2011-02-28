using System;
using MongoDB.Bson;

namespace ZeeBi.UI.Models
{
	public class Url
	{
		public string Id { get; set; }

		public string LongUrl { get; set; }

		public DateTime Created { get; set; }

		public int ClickCount { get; set; }

		public ObjectId UserId { get; set; }
	}

	public class PageView
	{
		public ObjectId Id { get; set; }

		public string UrlId { get; set; }

		public DateTime ViewedAt { get; set; }

		public string UserIp { get; set; }

		public string UserAgent { get; set; }

		public string OriginalUserAgent { get; set; }
	}
}
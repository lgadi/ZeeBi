using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ZeeBi.UI.Controllers;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.Services
{
	public class UrlsRepository
	{
		private readonly IdGenerator _idGenerator;

		public UrlsRepository()
		{
			_idGenerator = new IdGenerator();
		}

		public Url Add(string longUrl, string id = null, ObjectId userId = default(ObjectId))
		{
			var url = new Url
			{
				LongUrl = longUrl,
				Id = id,
				UserId = userId,
				Created = DateTime.UtcNow,
				ClickCount = 0
			};

			// Normalize and validate long URL
			url.LongUrl = new UrlNormalizer().Normalize(url.LongUrl);
			if (url.LongUrl == null) throw new InvalidUrlException(url.LongUrl);

			// Generate or validate short ID
			if (url.Id == null)
			{
				url.Id = _idGenerator.Generate();
			}
			else
			{
				if (_idGenerator.IsTaken(url.Id))
				{
					throw new IdAlreadyTakenException(url.Id);
				}
			}

			DB.Urls.Insert(url, SafeMode.FSyncTrue);
			return url;
		}

		public void RecordAnalytics(Url url, HttpRequestBase request)
		{

			DB.PageViews.Insert(new PageView
			{
				UrlId = url.Id,
				OriginalUserAgent = request.UserAgent,
				UserAgent = TransformUserAgent(request.UserAgent),
				UserIp = request.UserHostAddress,
				ViewedAt = DateTime.Now
			});

			DB.Urls.Update(Query.EQ("_id", url.Id), Update.Inc("ClickCount", 1));

		}

		private string TransformUserAgent(string userAgent)
		{
			if (string.IsNullOrEmpty(userAgent)) return "EMPTY";

			if (userAgent.Contains("Chrome")) return "Chrome";
			if (userAgent.Contains("IEMobile")) return "Mobile IE";
			if (userAgent.Contains("MSIE")) return "Internet Explorer";
			if (userAgent.Contains("Firefox")) return "Firefox";
			if (userAgent.Contains("Opera")) return "Opera";
			if (userAgent.Contains("iPhone")) return "iPhone";
			if (userAgent.Contains("BlackBerry")) return "BlackBerry";
			if (userAgent.Contains("Safari")) return "Safari";
			if (userAgent.Contains("Links")) return "Links";

			return "Other";
		}

		public bool IsAvailable(string id)
		{
			return !_idGenerator.IsTaken(id);
		}

		public Url FindById(string id)
		{
			return DB.Urls.FindOneById(id);
		}

		public IList<Url> FindByUser(ObjectId userId)
		{
			return DB.Urls.Find(Query.EQ("UserId", userId)).SetSortOrder(SortBy.Descending("Created")).ToList();
		}

		public int GetTotalUrlsShortened()
		{
			return DB.Urls.Count();
		}
	}
}
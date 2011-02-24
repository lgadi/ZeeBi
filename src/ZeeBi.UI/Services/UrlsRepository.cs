using System;
using System.Web;
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
		public void AddUrl(Url url)
		{
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

			url.LongUrl = new UrlNormalizer().Normalize(url.LongUrl);
			if (url.LongUrl == null) throw new InvalidUrlException(url.LongUrl);
			url.Created = DateTime.Now;
			url.ClickCount = 0;

			DB.Urls.Insert(url, SafeMode.FSyncTrue);
		}

		public void RecordAnalytics(Url url, HttpRequestBase request)
		{

			DB.PageViews.Insert(new PageView
			                    	{
				UrlId = url.Id,
				UserAgent = request.UserAgent,
				UserIp = request.UserHostAddress,
				ViewedAt = DateTime.Now
			});

			DB.Urls.Update(Query.EQ("_id", url.Id), Update.Inc("ClickCount", 1));

		}

		public bool IsAvailable(string id)
		{
			return !_idGenerator.IsTaken(id);
		}

		public Url FindOneById(string id)
		{
			return DB.Urls.FindOneById(id);
		}
	}
}
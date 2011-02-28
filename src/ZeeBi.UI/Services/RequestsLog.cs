using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using MongoDB.Bson;
using ZeeBi.UI.DataAccess;

namespace ZeeBi.UI.Services
{
	public static class RequestsLog
	{
		public static void SaveRequestData(HttpContextBase context)
		{
			var url = context.Request.Url.ToString();

			var data = new BsonDocumentWrapper(new
			{
				//_id = ObjectId.GenerateNewId(),
				at = DateTime.UtcNow,
				url,
				method = context.Request.HttpMethod,
				headers = From(context.Request.Headers),
				clientIp = context.Request.UserHostAddress
			}).ToBsonDocument();
			if (context.Request.Form.Count > 0)
			{
				data.Add("form", From(context.Request.Form));
			}
			var cookies = From(context.Response.Cookies);
			if (cookies.Length > 0)
				data.Add("cookies", BsonArray.Create((IEnumerable)cookies));
			var referrer = context.Request.UrlReferrer;
			if (referrer != null)
				data.Add("referrer", referrer.ToString());

			try
			{
				DB.Database.GetCollection("requestsLog").Insert(data);
			}
			catch
			{
			}
		}

		private static BsonDocument[] From(HttpCookieCollection cookies)
		{
			var docs = new List<BsonDocument>();
			try
			{
				foreach (HttpCookie httpCookie in cookies)
				{
					var doc = new BsonDocument
					{
						{"name", httpCookie.Name},
						{"domain", httpCookie.Domain},
						{"path", httpCookie.Path},
						{"values", From(httpCookie.Values)}
					};
					docs.Add(doc);
				}
			}
			catch
			{
			}
			return docs.ToArray();
		}
		private static BsonDocument From(NameValueCollection headers)
		{
			var doc = new BsonDocument();
			try
			{

				foreach (string key in headers)
				{
					var name = key ?? "@NULL";
					var values = (headers[key] ?? string.Empty).Split(',');
					if (values.Length == 1)
						doc.Add(name, BsonString.Create(values[0]));
					else
						doc.Add(name, BsonArray.Create((IEnumerable)values));
				}
			}
			catch
			{
			}
			return doc;
		}


	}
}
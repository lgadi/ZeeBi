using System;
using MongoDB.Driver.Builders;
using ZeeBi.UI.DataAccess;

namespace ZeeBi.UI.Services
{
	public class IdGenerator
	{
		public bool IsTaken(string id)
		{
			return DB.Urls.Find(Query.EQ("_id", id)).SetLimit(1).SetFields(new[] { "_id" }).Count() != 0;
		}

		public string Generate()
		{
    		string id;
    		do
    		{
    			id = GenerateRandomId();
    		} while (IsTaken(id));
    		return id;
    	}

    	private string GenerateRandomId()
    	{
    		return Guid.NewGuid().ToString().Substring(0, 6);
    	}
	}
}
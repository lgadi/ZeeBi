using MongoDB.Bson;
using MongoDB.Driver.Builders;
using ZeeBi.UI.DataAccess;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.Services
{
	public class UsersRepository
	{
		public User FindByOpenId(string openId)
		{
			return DB.Users.FindOne(Query.EQ("OpenId", openId));		
		}

		public void Add(User user)
		{
			DB.Users.Insert(user);
		}

		public User FindById(string id)
		{
			return DB.Users.FindOneById(new ObjectId(id));
		}
	}
}
using MongoDB.Driver;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.DataAccess
{
	public static class DB
	{
		public static MongoServer Server { get { return MongoServer.Create("mongodb://localhost/zeebi"); } }
		public static MongoDatabase Database { get { return Server.GetDatabase("zeebi"); } }
		public static MongoCollection<Url> Urls { get { return Database.GetCollection<Url>("urls"); } }
		public static MongoCollection<PageView> PageViews { get { return Database.GetCollection<PageView>("pageviews"); } }
		public static MongoCollection<User> Users { get { return Database.GetCollection<User>("users"); } }
	}
}
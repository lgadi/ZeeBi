using MongoDB.Driver;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.DataAccess
{
	public static class DB
	{
		public static MongoServer Server { get { return MongoServer.Create("mongodb://localhost/zeebi"); } }
		public static MongoDatabase Zeebi { get { return Server.GetDatabase("zeebi"); } }
		public static MongoCollection<Url> Urls { get { return Zeebi.GetCollection<Url>("urls"); } }
	}
}
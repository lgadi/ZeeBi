using MongoDB.Bson;

namespace ZeeBi.UI.Models
{
	public class User
	{
		public ObjectId Id { get; set; }
		public string UserName { get; set; }
		public string OpenId { get; set; }
		public string Friendly { get; set; }
		public string Email { get; set; }
	}
}
using System.Collections.Generic;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.ViewModels.Urls
{
	public class HomeViewModel
	{
		public string LongUrl { get; set; }
		public string Id { get; set; }

		public IList<Url> MyUrls { get; set; }
	}
}
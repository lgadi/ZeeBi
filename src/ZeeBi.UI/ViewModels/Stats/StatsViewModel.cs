using System;
using System.Collections.Generic;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.ViewModels.Stats
{
	public class StatsViewModel
	{
		public Url Url { get; set; }
		public int PageViewCount { get; set; }
		public ICollection<KeyValuePair<string, int>> PageViewsByUserAgent { get; set; }
		public ICollection<KeyValuePair<DateTime, int>> PageViewsByDate { get; set; }
	}
}
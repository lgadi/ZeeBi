using System;
using System.Collections.Generic;
using ZeeBi.UI.Models;

namespace ZeeBi.UI.ViewModels.Stats
{
	public class StatsViewModel
	{
		public Url Url { get; set; }
		public int PageViewCount { get; set; }
		public IDictionary<string, int> PageViewsByUserAgent { get; set; }
		public Dictionary<DateTime, int> PageViewsByDate { get; set; }
	}
}
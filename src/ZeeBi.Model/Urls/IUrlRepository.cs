using System.Collections.Generic;

namespace ZeeBi.Model.Urls
{
	public interface IUrlRepository
	{
		Url Create(string longUrl);
		Url FindById(string id);
	}
}
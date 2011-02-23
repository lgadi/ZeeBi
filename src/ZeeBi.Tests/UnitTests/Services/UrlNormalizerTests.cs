using NUnit.Framework;
using ZeeBi.UI.Services;

namespace ZeeBi.Tests.UnitTests.Services
{
	[TestFixture]
	public class UrlNormalizerTests
	{
		private UrlNormalizer _target;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_target = new UrlNormalizer();
		}

		[Test]
		public void ValidUrlWithSchema_IsValid()
		{
			var url = "http://www.example.com:8080/path/to/resource.extension?p1=v&p2";
			Assert.That(_target.IsValid(url), Is.True);
		}

		[Test]
		public void ValidUrlWithoutPath_IsNormalizedWithTrailingSlash()
		{
			var url = "http://www.example.com";
			Assert.That(_target.Normalize(url), Is.EqualTo(url + "/"));
		}

		[Test]
		public void ValidUrlWithoutSchema_IsNormalizedToHttp()
		{
			var url = "www.example.com:8080/path/to/resource.extension?p1=v&p2";
			Assert.That(_target.Normalize(url), Is.EqualTo("http://" + url));
		}

		[Test]
		public void InvalidUrl_IsNormalizedToNull()
		{
			var url = "www..example.com";
			Assert.That(_target.Normalize(url), Is.Null);
		}
	}
}
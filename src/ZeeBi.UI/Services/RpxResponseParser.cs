using ZeeBi.UI.Models;

namespace ZeeBi.UI.Services
{
	using System;
	using System.Linq;
	using System.Xml;
	using System.Xml.Linq;
	

	
		public enum RpxReponseStatus
		{
			Ok,
			Fail
		}

		public class RpxResponseParser
		{
			#region Fields

			readonly XElement _response;

			#endregion

			#region Properties

			public RpxReponseStatus Status { get; set; }

			#endregion

			#region Constructor

			public RpxResponseParser(XmlNode response)
			{
				_response = ToXElement(response);
				Status = ParseStatus();
			}

			#endregion

			#region Public Methods

			public User BuildUser()
			{
				return new User
				{
					UserName = GetUserName(),
					OpenId = GetOpenId(),
					Friendly = GetDisplayName(),
					Email = GetEmail()
				};
			}

			public string GetEmail()
			{
				var email = _response.Descendants("profile").Select(p => (string)p.Element("email")).SingleOrDefault();
				return email;
			}

			public string GetDisplayName()
			{
				var displayName = _response.Descendants("profile").Select(p => (string)p.Element("displayName")).SingleOrDefault();
				return displayName;
			}

			public string GetUserName()
			{
				var userName = _response.Descendants("profile").Select(p => (string)p.Element("preferredUsername")).SingleOrDefault();
				return userName;
			}

			public string GetOpenId()
			{
				var userName = _response.Descendants("profile").Select(p => (string)p.Element("identifier")).SingleOrDefault();
				return userName;
			}

			#endregion

			#region Private Methods

			private static XElement ToXElement(XmlNode xml)
			{
				return XElement.Parse(xml.OuterXml);
			}

			private RpxReponseStatus ParseStatus()
			{
				var status = (string)_response.Attribute("stat");
				return (RpxReponseStatus)Enum.Parse(typeof(RpxReponseStatus), status, true /*ignoreCase*/);
			}

			#endregion
		}
}
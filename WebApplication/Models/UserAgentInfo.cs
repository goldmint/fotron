using System;
using System.Text;

namespace Fotron.WebApplication.Models {

	public class UserAgentInfo {

		private string _agentRaw;

		public string Ip { get; set; }
		public System.Net.IPAddress IpObject { get; set; }
		public string Agent {
			get => FormatUserAgentString(_agentRaw);
			set => _agentRaw = value;
		}

		public static string FormatUserAgentString(string uas) {
			try {
				var sb = new StringBuilder();

				var parser = UAParser.Parser.GetDefault();
				var ua = parser.Parse(uas);

				var knownDeviceFamily = ua.Device.Family != "Other";

				return sb
					.Append(knownDeviceFamily ? ua.Device.Family: "")
					.Append("/")
					.Append(ua.OS.Family)
					.Append("/")
					.Append(ua.UserAgent.Family)
					.ToString()
					.Trim('/')
					;
			}
			catch (Exception) {
				return uas;
			}
		}
	}
}

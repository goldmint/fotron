using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fotron.Common.WebRequest;

namespace Fotron.WebApplication.Core {

	public static class Recaptcha {

		public static async Task<bool> Verify(string secret, string response, string ip) {

			var fields = new Parameters()
				.Set("secret", secret)
				.Set("response", response)
				.Set("remoteip", ip)
			;

			var result = new Dictionary<string, string>();

			using (var req = new Request(null)) {
				await req
					.AcceptJson()
					.BodyForm(fields)
					.OnResult(async (res) => {
						await res.ToJson(result);
					})
					.SendPost("https://www.google.com/recaptcha/api/siteverify", TimeSpan.FromSeconds(30))
				;
			}


			return result.GetValueOrDefault("success", null) == "true";
		}
	}
}

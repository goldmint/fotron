using Fotron.Common;
using Fotron.Common.WebRequest;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fotron.Common.Extensions;

namespace Fotron.CoreLogic.Services.KYC.Impl {

	public class ShuftiProKycProvider : IKycProvider {

		private readonly ShuftiProOptions _opts;
		private readonly ILogger _logger;

		public ShuftiProKycProvider(Action<ShuftiProOptions> setup, LogFactory logFactory) {
			_opts = new ShuftiProOptions() { };
			setup(_opts);
			_logger = logFactory.GetLoggerFor(this);
		}

		// ---

		private string CalcSignature(Dictionary<string,string> fields) {
			var pairs = fields.ToList();
			pairs.Sort((a, b) => a.Key.CompareTo(b.Key));

			var values = from x in pairs select x.Value;

			return Hash.SHA256(
				string.Join("", values) + _opts.ClientSecret
			);
		}

		private bool CheckCallbackSignature(string rawJson) {
			var fields = new Dictionary<string, string>();
			if (!Json.ParseInto(rawJson, fields)) return false;

			var pairs = fields.ToList();
			var values = from x in pairs where x.Key != "signature" select x.Value;
			if (values.Count() == 0) return false;

			var clientSign = fields.GetValueOrDefault("signature");
			if (clientSign == null) return false;

			return Hash.SHA256(
				string.Join("", values) + _opts.ClientSecret
			) == clientSign?.ToLower();
		}

		internal class ServiceJsonResponse {

			public string status_code { get; set; }
			public string message { get; set; }
			public string reference { get; set; }
			public string signature { get; set; }
		}

		// ---

		public async Task<string> GetRedirect(UserData user, string ticketId, string userRedirectUrl, string callbackUrl) {
			var ret = (string)null;

			var fields = new Parameters()
				.Set("method", "")
				.Set("client_id", _opts.ClientId)
				.Set("reference", ticketId)
				.Set("first_name", user.FirstName)
				.Set("last_name", user.LastName)
				.Set("country", user.CountryCode)
				.Set("dob", user.DoB.ToString("yyyy-MM-dd"))
				.Set("phone_number", "+" + user.PhoneNumber.Trim('+'))
				.Set("callback_url", callbackUrl)
				.Set("redirect_url", userRedirectUrl)
				.Set("background_checks", "0")
			;
			fields.Set("signature", CalcSignature(fields.GetDictionary()));

			using (var req = new Request(_logger)) {
				await req
					.AcceptJson()
					.BodyForm(fields)
					.OnResult(async (res) => {
						var raw = await res.ToRawString();

						var result = new ServiceJsonResponse();
						if (Json.ParseInto(raw, result) && CheckCallbackSignature(raw)) {
							if (result.status_code == "SP2") {
								ret = result.message;
							} else {
								_logger?.Error("Failed to get redirect: {0} with message: {1}", result.status_code, result.message);
							}
						} else {
							_logger?.Error("Failed to parse response or invalid signature");
						}
					})
					.SendPost("https://api.shuftipro.com/")
				;
			}

			return ret;
		}

		public async Task<CallbackResult> OnServiceCallback(HttpRequest request) {
			var ret = new CallbackResult() {
				OverallStatus = VerificationStatus.Pending,
			};

			try {
				var result = new ServiceJsonResponse();
				var resultDict = new Dictionary<string, string>();

				var raw = "";
				using (var reader = new StreamReader(request.Body)) {
					raw = await reader.ReadToEndAsync();
				}

				if (!Json.ParseInto(raw, result) || result.signature == null) {
					throw new Exception("Failed to parse response");
				}

				if (!CheckCallbackSignature(Json.Stringify(result))) {
					throw new Exception("Invalid signature");
				}

				var finalStatus =
					result.status_code == "SP0" || // verified
					result.status_code == "SP1" // not verified
				;

				if (finalStatus) {
					ret.OverallStatus = result.status_code == "SP1"? VerificationStatus.Verified: VerificationStatus.NotVerified;
					ret.TicketId = result.reference;
					ret.ServiceStatus = result.status_code;
					ret.ServiceMessage = result.message;
				}

				_logger?.Info("Callback code {0} for ref {1}: {2}", result.status_code, result.reference, result.message);

			} catch (Exception e) {
				ret.OverallStatus = VerificationStatus.Fail;
				_logger?.Info(e, "Callback failure");
			}

			return ret;
		}
	}

	public sealed class ShuftiProOptions {

		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
	}
}

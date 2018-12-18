using Fotron.Common;
using Fotron.Common.WebRequest;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fotron.Common.Extensions;

namespace Fotron.CoreLogic.Services.KYC.Impl {

	public sealed class DebugKycProvider : IKycProvider {

		private readonly ILogger _logger;

		public DebugKycProvider(LogFactory logFactory) {
			_logger = logFactory.GetLoggerFor(this);
		}

		// ---

		public async Task<string> GetRedirect(UserData user, string ticketId, string userRedirectUrl, string callbackUrl) {

			var nowTime = DateTime.UtcNow;
			var verified = nowTime.Second < 30;

			var fields = new Parameters()
				.Set("status_code", verified? "1": "0")
				.Set("message", $"Seconds - { nowTime.Second } - { verified }")
				.Set("reference", ticketId)
			;

			using (var req = new Request(_logger)) {
				await req
					.AcceptJson()
					.BodyJson(fields.ToJson())
					.SendPost(callbackUrl)
				;
			}

			return userRedirectUrl;
		}

		public async Task<CallbackResult> OnServiceCallback(HttpRequest request) {
			var ret = new CallbackResult() {
				OverallStatus = VerificationStatus.Pending,
			};

			try {
				var result = new Dictionary<string, string>();

				var raw = "";
				using (var reader = new StreamReader(request.Body)) {
					raw = await reader.ReadToEndAsync();
				}

				if (!Json.ParseInto(raw, result)) {
					throw new Exception("Failed to parse response");
				}

				ret.OverallStatus = result["status_code"] == "1" ? VerificationStatus.Verified : VerificationStatus.NotVerified;
				ret.TicketId = result["reference"];
				ret.ServiceStatus = result["status_code"];
				ret.ServiceMessage = result["message"];

				_logger?.Info($"Callback code={ result["status_code"] } for ref { result["reference"] }: { result["message"] }");
			}
			catch (Exception e) {
				ret.OverallStatus = VerificationStatus.Fail;
				_logger?.Info(e, "Callback failure");
			}

			return ret;
		}
	}
}

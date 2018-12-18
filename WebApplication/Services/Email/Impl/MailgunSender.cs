using Fotron.Common;
using Fotron.Common.Extensions;
using Fotron.Common.WebRequest;
using Microsoft.EntityFrameworkCore.Internal;
using NLog;
using System;
using System.Threading.Tasks;

namespace Fotron.WebApplication.Services.Email.Impl {

	public sealed class MailGunSender : IEmailSender {

		private readonly AppConfig _appConfig;
		private readonly ILogger _logger;

		public MailGunSender(AppConfig appConfig, LogFactory logFactory) {
			_logger = logFactory.GetLoggerFor(this);
			_appConfig = appConfig;
		}

		public async Task<bool> Send(string[] recipients, string subject, string body) {
			
			if ((recipients?.Length ?? 0) == 0) {
				return true;
			}

			var ret = false;
			var url = _appConfig.Services.MailGun.Url.TrimEnd('/', ' ') + "/" + _appConfig.Services.MailGun.DomainName.TrimEnd('/', ' ') + "/messages";

			var postParams = new Parameters()
				.Set("from", _appConfig.Services.MailGun.Sender)
				.Set("to", $"<{recipients.Join(">,<")}>")
				.Set("subject", subject)
				.Set("text", body)
			;

			using (var req = new Request(_logger)) {
				await req
					.AcceptJson()
					.AuthBasic("api:" + _appConfig.Services.MailGun.Key, true)
					.BodyForm(postParams)
					.OnResult(async (res) => {
						if (res.GetHttpStatus() != System.Net.HttpStatusCode.OK) {
							_logger?.Error("Message has not been sent to <{0}> with subject `{1}`. Status {2}. Raw: `{3}`", recipients.Join(","), subject, res.GetHttpStatus(), await res.ToRawString());
						} else {
							ret = true;
						}
					})
					.SendPost(url, TimeSpan.FromSeconds(90))
				;
			}

			return ret;
		}
	}
}

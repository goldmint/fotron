using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Fotron.WebApplication.Core.Policies;

namespace Fotron.WebApplication.Controllers.v1 {

	[Route("api/v1/callback")]
	public class CallbackController : BaseController {

		/// <summary>
		/// Redirect via GET request
		/// </summary>
		[AnonymousAccess]
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpGet, Route("redirect", Name = "CallbackRedirect")]
		public IActionResult RedirectGet(string to) {
			if (to != null) {
				to = System.Web.HttpUtility.UrlDecode(to);
				return Redirect(to);
			}
			return LocalRedirect("/");
		}

		/// <summary>
		/// Redirect via POST request
		/// </summary>
		[AnonymousAccess]
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPost, Route("redirect", Name = "CallbackRedirect")]
		public IActionResult RedirectPost(string to) {
			if (to != null) {
				to = System.Web.HttpUtility.UrlDecode(to);
				return Redirect(to);
			}
			return LocalRedirect("/");
		}

		///// <summary>
		///// Callback from ShuftiPro service. This is not user redirect url
		///// </summary>
		//[AnonymousAccess]
		//[HttpPost, Route("shuftipro", Name = "CallbackShuftiPro")]
		//[ApiExplorerSettings(IgnoreApi = true)]
		//public async Task<IActionResult> ShuftiPro() {

		//	//if (secret == AppConfig.Services.ShuftiPro.CallbackSecret) {

		//		var check = await KycExternalProvider.OnServiceCallback(HttpContext.Request);
		//		if (check.IsFinalStatus) {

		//			var ticket = await (
		//				from t in DbContext.KycShuftiProTicket
		//				where t.ReferenceId == check.TicketId && t.TimeResponded == null
		//				select t
		//			)
		//				.Include(tickt => tickt.User)
		//				.ThenInclude(user => user.UserVerification)
		//				.FirstOrDefaultAsync()
		//			;

		//			if (ticket != null) {

		//				var userVerified = check.OverallStatus == CoreLogic.Services.KYC.VerificationStatus.Verified;

		//				ticket.IsVerified = userVerified;
		//				ticket.CallbackStatusCode = check.ServiceStatus;
		//				ticket.CallbackMessage = check.ServiceMessage;
		//				ticket.TimeResponded = DateTime.UtcNow;

		//				await DbContext.SaveChangesAsync();
		//			}
		//		}
		//	//}

		//	return Ok();
		//}
	}
}
using AutoMapper;
using Fotron.Common;
using Fotron.Common.Extensions;
using Fotron.CoreLogic.Services.Blockchain.Ethereum;
using Fotron.DAL;
using Fotron.WebApplication.Core.Tokens;
using Fotron.WebApplication.Models;
using Fotron.WebApplication.Services.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fotron.WebApplication.Controllers.v1 {

	public abstract class BaseController : Controller {

		protected AppConfig AppConfig { get; private set; }
		protected IHostingEnvironment HostingEnvironment { get; private set; }
		protected ILogger Logger { get; private set; }
		protected ApplicationDbContext DbContext { get; private set; }
		//protected IMutexHolder MutexHolder { get; private set; }
		protected SignInManager<DAL.Models.Identity.User> SignInManager { get; private set; }
		protected UserManager<DAL.Models.Identity.User> UserManager { get; private set; }
		//protected IKycProvider KycExternalProvider { get; private set; }
		protected IEmailSender EmailSender { get; private set; }
		protected IEthereumReader EthereumObserver { get; private set; }

	    protected IMapper Mapper { get; private set; }

        protected BaseController() { }

		[NonAction]
		private void InitServices(IServiceProvider services) {

			Logger = services.GetLoggerFor(this.GetType());
			AppConfig = services.GetRequiredService<AppConfig>();
			HostingEnvironment = services.GetRequiredService<IHostingEnvironment>();
			DbContext = services.GetRequiredService<ApplicationDbContext>();
			//MutexHolder = services.GetRequiredService<IMutexHolder>();
			SignInManager = services.GetRequiredService<SignInManager<DAL.Models.Identity.User>>();
			UserManager = services.GetRequiredService<UserManager<DAL.Models.Identity.User>>();
			//KycExternalProvider = services.GetRequiredService<IKycProvider>();
			EmailSender = services.GetRequiredService<IEmailSender>();
			EthereumObserver = services.GetRequiredService<IEthereumReader>();
		    Mapper = services.GetRequiredService<IMapper>();
        }

		// ---

		[NonAction]
		public override void OnActionExecuted(ActionExecutedContext context) {
			InitServices(context?.HttpContext?.RequestServices);
			base.OnActionExecuted(context);
		}

		[NonAction]
		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);
		}

		[NonAction]
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
			InitServices(context?.HttpContext?.RequestServices);
			await base.OnActionExecutionAsync(context, next);
		}

		// ---

		[NonAction]
		public string MakeAppLink(JwtAudience audience, string fragment) {

			var appUri = (string)null;
			if (audience == JwtAudience.Cabinet) {
				appUri = AppConfig.Apps.Cabinet.Url;
			}
			else if (audience == JwtAudience.Dashboard) {
				appUri = AppConfig.Apps.Dashboard.Url;
			}
			else {
				throw new NotImplementedException("Audience is not implemented. Could not create app link");
			}

			var uri = new UriBuilder(new Uri(appUri));
			if (fragment != null) {
				uri.Fragment = fragment;
			}
			return uri.ToString();
		}

		[NonAction]
		protected bool IsUserAuthenticated() {
			return HttpContext.User?.Identity.IsAuthenticated ?? false;
		}

		[NonAction]
		protected JwtAudience? GetCurrentAudience() {
			var audStr = HttpContext.User.Claims.FirstOrDefault(_ => _.Type == "aud")?.Value;
			if (audStr != null) {
				if (Enum.TryParse(audStr, true, out JwtAudience aud)) {
					return aud;
				}
			}
			return null;
		}

		[NonAction]
		protected long GetCurrentRights() {
			var rightsStr = HttpContext.User.Claims.FirstOrDefault(_ => _.Type == JWT.ErRightsField)?.Value;
			if (rightsStr != null) {
				if (long.TryParse(rightsStr, out long rights)) {
					return rights;
				}
			}
			return 0;
		}


		[NonAction]
		protected async Task<DAL.Models.Identity.User> GetUserFromDb() {
			if (IsUserAuthenticated()) {
				var name = UserManager.NormalizeKey(HttpContext.User.Identity.Name);
				return await DbContext.Users
						.Include(_ => _.UserOptions)
						.AsTracking()
						.FirstAsync(user => user.NormalizedUserName == name)
					;
			}
			return null;
		}

		[NonAction]
		protected UserAgentInfo GetUserAgentInfo() {

			var ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

#if DEBUG
			if (HostingEnvironment.IsDevelopment() && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DEBUG_CUSTOM_IP"))) {
				if (System.Net.IPAddress.TryParse(Environment.GetEnvironmentVariable("DEBUG_CUSTOM_IP"), out System.Net.IPAddress customIp)) {
					ip = customIp.MapToIPv4().ToString();
				}
			}
#endif

			// ip object
			var ipObj = System.Net.IPAddress.Parse(ip);

			// agent
			var agent = "Unknown";
			if (HttpContext.Request.Headers.TryGetValue("User-Agent", out var agentParsed)) {
				agent = agentParsed.ToString();
			}

			return new UserAgentInfo() {
				Ip = ip,
				IpObject = ipObj,
				Agent = agent,
			};
		}

		[NonAction]
		protected Locale GetUserLocale() {
			if (
				HttpContext.Request.Headers.TryGetValue("GM-LOCALE", out var localeHeader) &&
				!string.IsNullOrWhiteSpace(localeHeader.ToString()) &&
				Enum.TryParse(localeHeader.ToString(), true, out Locale localeEnum)
			) {
				return localeEnum;
			}
			return Locale.En;
		}

		[NonAction]
		protected async Task<UserTier> GetUserTier() {
			var user = await GetUserFromDb();
			return CoreLogic.User.GetTier(user);
		}
	}
}

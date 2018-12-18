using Fotron.Common;
using Fotron.WebApplication.Core;
using Fotron.WebApplication.Core.Policies;
using Fotron.WebApplication.Core.Response;
using Fotron.WebApplication.Core.Tokens;
using Fotron.WebApplication.Models.API.v1.OAuthModels;
using Fotron.WebApplication.Services.OAuth;
using Fotron.WebApplication.Services.OAuth.Impl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Fotron.WebApplication.Controllers.v1 {

	[Route("api/v1/oauth")]
	public class OAuthController : BaseController {

		/// <summary>
		/// Create redirect
		/// </summary>
		[AnonymousAccess]
		[HttpGet, Route("google")]
		[ProducesResponseType(typeof(RedirectView), 200)]
		public async Task<APIResponse> Google() {

			var provider = HttpContext.RequestServices.GetRequiredService<GoogleProvider>();

			return APIResponse.Success(
				new RedirectView() {
					Redirect = await provider.GetRedirect(
						Url.Link("OAuthGoogleCallback", new { }),
						null
					),
				}
			);
		}

		/// <summary>
		/// On callback
		/// </summary>
		[AnonymousAccess]
		[HttpGet, Route("googleCallback", Name = "OAuthGoogleCallback")]
		[ProducesResponseType(302)]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> GoogleCallback(string error, string code, string state) {
			try {

				if (!string.IsNullOrWhiteSpace(error) || string.IsNullOrWhiteSpace(code)) {
					throw new Exception("Invalid callback result");
				}

				var provider = HttpContext.RequestServices.GetRequiredService<GoogleProvider>();
				var userInfo = await provider.GetUserInfo(
					Url.Link("OAuthGoogleCallback", new { }),
					state,
					code
				);

				return await ProcessOAuthCallback(
					LoginProvider.Google,
					userInfo
				);
			}
			catch {
				return Redirect("/");
			}
		}

		// ---

		[NonAction]
		public async Task<RedirectResult> ProcessOAuthCallback(LoginProvider provider, UserInfo userInfo) {
			Logger.Info($"User {userInfo.Id} - login attempt");

			var audience = JwtAudience.Cabinet;
			var userLocale = GetUserLocale();

			// find user with this ext login
			var user = await UserManager.FindByLoginAsync(provider.ToString(), userInfo.Id);
			
			// exists
			if (user != null) {
				Logger.Info($"User {userInfo.Id} - account {user.Id} found");

				// try to sign in
				var signResult = await SignInManager.CanSignInAsync(user);

				// can't sign in
				if (!signResult) {
					Logger.Info($"User {userInfo.Id} - can't sign in");

					// load options
					await DbContext.Entry(user).Reference(_ => _.UserOptions).LoadAsync();
	
				}
				else {
					Logger.Info($"User {userInfo.Id} - can sign in");

					// check rights
					var accessRightsMask = Core.UserAccount.ResolveAccessRightsMask(HttpContext.RequestServices, audience, user);
					if (accessRightsMask != null) {
						var agent = GetUserAgentInfo();

						//// notification
						//await EmailComposer.FromTemplate(await TemplateProvider.GetEmailTemplate(EmailTemplate.SignedIn, userLocale))
						//	.ReplaceBodyTag("IP", agent.Ip)
						//	.Initiator(agent.Ip, agent.Agent, DateTime.UtcNow)
						//	.Send(user.Email, user.UserName, EmailQueue)
						//;

						// activity
						var userActivity = CoreLogic.User.CreateUserActivity(
							user: user,
							type: Common.UserActivityType.Auth,
							comment: "Signed in with social network",
							ip: agent.Ip,
							agent: agent.Agent,
							locale: userLocale
						);
						DbContext.UserActivity.Add(userActivity);
						await DbContext.SaveChangesAsync();

						// tfa required
						if (user.TwoFactorEnabled) {
							Logger.Info($"User {userInfo.Id} - 2FA required");

							var tokenForTfa = JWT.CreateAuthToken(
								appConfig: AppConfig,
								user: user,
								audience: audience,
								area: JwtArea.Tfa,
								rightsMask: accessRightsMask.Value
							);
							return Redirect(
								this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteOAuthTfaPage.Replace(":token", tokenForTfa))
							);
						}
						
						// new jwt salt
						UserAccount.GenerateJwtSalt(user, audience);
						DbContext.SaveChanges();

						Logger.Info($"User {userInfo.Id} - signed in");

						// ok
						var token = JWT.CreateAuthToken(
							appConfig: AppConfig,
							user: user,
							audience: audience,
							area: JwtArea.Authorized,
							rightsMask: accessRightsMask.Value
						);
						return Redirect(
							this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteOAuthAuthorized.Replace(":token", token))
						);
					}
					else {
						Logger.Info($"User {userInfo.Id} - hasn't rights");
					}
				}

				Logger.Info($"User {userInfo.Id} - failure 1");

				// never should get here
				return Redirect("/");
			}

			// doesnt exist yet
			else {
				Logger.Info($"User {userInfo.Id} - account creation");

				// try create and sign in
				var cuaResult = await Core.UserAccount.CreateUserAccount(HttpContext.RequestServices, userInfo.Email);
				if (cuaResult.User != null) {
					Logger.Info($"User {userInfo.Id} - account {cuaResult.User.Id} created");

					// user created and external login attached
					if (await CreateExternalLogin(cuaResult.User, provider, userInfo)) {
						Logger.Info($"User {userInfo.Id} - external login created");

						var accessRightsMask = Core.UserAccount.ResolveAccessRightsMask(HttpContext.RequestServices, audience, cuaResult.User);
						if (accessRightsMask != null) {

							Logger.Info($"User {userInfo.Id} - signed in");

							// ok
							var token = JWT.CreateAuthToken(
								appConfig: AppConfig,
								user: cuaResult.User,
								audience: audience,
								area: JwtArea.Authorized,
								rightsMask: accessRightsMask.Value
							);
							return Redirect(
								this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteOAuthAuthorized.Replace(":token", token))
							);
						}
					}

					Logger.Info($"User {userInfo.Id} - failure 2");

					// failed
					return Redirect("/");
				}

				Logger.Info($"User {userInfo.Id} - failure 3");

				// redirect to error OR email input
				return Redirect(
					this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteEmailTaken)
				);
			}
		}

		[NonAction]
		private async Task<bool> CreateExternalLogin(DAL.Models.Identity.User user, LoginProvider provider, UserInfo userInfo) {

			// attach login
			var res = await UserManager.AddLoginAsync(user, new UserLoginInfo(provider.ToString(), userInfo.Id, provider.ToString()));

			// sign in
			if (res.Succeeded) {
				return true;
			}
			return false;
		}
	}
}

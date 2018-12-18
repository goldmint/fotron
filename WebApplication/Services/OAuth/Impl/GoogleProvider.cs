using Fotron.Common;
using Fotron.Common.WebRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fotron.WebApplication.Core.Tokens;

namespace Fotron.WebApplication.Services.OAuth.Impl {

	public class GoogleProvider : IOAuthProvider {

		private AppConfig _appConfig;
		private string _clientId;
		private string _clientSecret;

		public GoogleProvider(AppConfig appConfig) {
			_appConfig = appConfig;

			_clientId = appConfig.Auth.Google.ClientId;
			_clientSecret = appConfig.Auth.Google.ClientSecret;
		}

		public Task<string> GetRedirect(string callbackUrl, long? userId) {

			var state = JWT.CreateSecurityToken(
				appConfig: _appConfig, 
				entityId: userId?.ToString() ?? Guid.NewGuid().ToString("N"), 
				securityStamp: "",
				audience: JwtAudience.Cabinet,
				area: JwtArea.OAuth,
				validFor: TimeSpan.FromMinutes(5),
				optClaims: new[] { new System.Security.Claims.Claim("google", "true") }
			);

			var paramz = new Parameters()
				.Set("scope", "https://www.googleapis.com/auth/userinfo.email")
				.Set("access_type", "offline")
				.Set("include_granted_scopes", "true")
				.Set("state", state)
				.Set("redirect_uri", callbackUrl)
				.Set("include_granted_scopes", "true")
				.Set("response_type", "code")
				.Set("client_id", _clientId)
			;

			return Task.FromResult("https://accounts.google.com/o/oauth2/v2/auth?" + paramz.ToUrlEncoded());
		}

		public async Task<UserInfo> GetUserInfo(string callbackUrl, string oauthState, string oauthCode) {
			
			if (string.IsNullOrWhiteSpace(oauthCode)) {
				throw new ArgumentException("Empty oauth code");
			}

			if (! await JWT.IsValid(
				appConfig: _appConfig,
				jwtToken: oauthState,
				expectedAudience: JwtAudience.Cabinet,
				expectedArea: JwtArea.OAuth, 
				validStamp: (jwt, id) => Task.FromResult("") )
			) {
				throw new ArgumentException("Invalid oauth state");
			}

			var raw = "";
			var tokenUrl = "https://accounts.google.com/o/oauth2/token";
			var infoUrl = "https://www.googleapis.com/oauth2/v1/userinfo";

			// get access token

			var atParams = new Parameters()
				.Set("code", oauthCode)
				.Set("client_secret", _clientSecret)
				.Set("client_id", _clientId)
				.Set("redirect_uri", callbackUrl)
				.Set("grant_type", "authorization_code")
			;

			var atResult = new Dictionary<string, string>();

			using (var atRequest = new Request(null)) {
				await atRequest
					.AcceptJson()
					.BodyForm(atParams)
					.OnResult(async (res) => {
						raw = await res.ToRawString();

						if (res.GetHttpStatus() == System.Net.HttpStatusCode.OK) {
							Json.ParseInto(raw, atResult);
						}
						else {
							throw new Exception("Status not 200 #1");
						}
					})
					.SendPost(tokenUrl)
				;
			}

			if (!atResult.ContainsKey("access_token")) {
				throw new Exception("Access token is empty");
			}

			// query info

			var infoParams = new Parameters()
				.Set("access_token", atResult["access_token"])
				.Set("token_type", atResult["token_type"])
			;

			var infoResult = new Dictionary<string, string>();

			using (var req = new Request(null)) {
				await req
					.AcceptJson()
					.Query(infoParams)
					.OnResult(async (res) => {
						raw = await res.ToRawString();

						if (res.GetHttpStatus() == System.Net.HttpStatusCode.OK) {
							Json.ParseInto(raw, infoResult);
						}
						else {
							throw new Exception("Status not 200 #2");
						}
					})
					.SendGet(infoUrl)
				;
			}

			// extract
			if (!infoResult.ContainsKey("id") || !infoResult.ContainsKey("email")) {
				throw new Exception("User info is empty");
			}

			return new UserInfo() {
				Id = infoResult["id"],
				Email = infoResult["email"],
			};
		}
	}
}

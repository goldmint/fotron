using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fotron.Common;
using Fotron.DAL;
using Fotron.DAL.Models.Identity;
using Fotron.WebApplication.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Fotron.WebApplication.Core.Tokens {

	public static class JWT {

		public const string ErAreaField = "er_area";
		public const string ErIdField = "er_id";
		public const string ErSecurityStampField = "er_sstamp";
		public const string ErRightsField = "er_rights";

		// ---

		/// <summary>
		/// Default token validation parameters
		/// </summary>
		public static TokenValidationParameters ValidationParameters(AppConfig appConfig) {

			var auds = (from a in appConfig.Auth.Jwt.Audiences select a.Audience.ToLower()).ToArray();

			return new TokenValidationParameters() {
				NameClaimType = ErIdField,
				RoleClaimType = ErRightsField,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = CreateJwtSecurityKey(appConfig.Auth.Jwt.Secret),
				ValidateIssuer = true,
				ValidIssuer = appConfig.Auth.Jwt.Issuer,
				ValidateAudience = true,
				ValidAudiences = auds,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero,
			};
		}

		/// <summary>
		/// Unique JWT subject
		/// </summary>
		private static string UniqueId(string salt) {
			return Hash.SHA256(Guid.NewGuid().ToString("N") + salt);
		}

		/// <summary>'fxdzs0987н6е
		/// Get user's security stamp from DB and hash it
		/// </summary>
		private static string ObtainSecurityStamp(string input) {
			return Hash.SHA256(Hash.SHA256(input));
		}

		/// <summary>
		/// Audience settings
		/// </summary>
		private static AppConfig.AuthSection.JwtSection.AudienceSection GetAudienceSettings(AppConfig appConfig, JwtAudience audience) {
			return (from a in appConfig.Auth.Jwt.Audiences where a.Audience == audience.ToString() select a).FirstOrDefault();
		}
		
		/// <summary>
		/// Make a secure key from main secret phrase
		/// </summary>
		public static SymmetricSecurityKey CreateJwtSecurityKey(string secret) {
			return new SymmetricSecurityKey(
				System.Text.Encoding.UTF8.GetBytes(
					secret
				)
			);
		}

		// ---

		/// <summary>
		/// Make a token for specified user with specified state
		/// </summary>
		public static string CreateAuthToken(AppConfig appConfig, JwtAudience audience, JwtArea area, User user, long rightsMask) {

			var now = DateTime.UtcNow;
			var uniqueness = UniqueId(appConfig.Auth.Jwt.Secret);
			var audienceSett = GetAudienceSettings(appConfig, audience);
			var jwtSalt = UserAccount.CurrentJwtSalt(user, audience);

			var claims = new[] {

				// jw main fields
				new Claim(JwtRegisteredClaimNames.Sub, uniqueness),
				new Claim(JwtRegisteredClaimNames.Jti, uniqueness),
				new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
				
				// gm fields
				new Claim(ErSecurityStampField, ObtainSecurityStamp(jwtSalt)),
				new Claim(ErIdField, user.UserName),
				new Claim(ErRightsField, rightsMask.ToString()),
				new Claim(ErAreaField, area.ToString().ToLower()),
			};

			var claimIdentity = new ClaimsIdentity(
				claims,
				JwtBearerDefaults.AuthenticationScheme
			);

			var creds = new SigningCredentials(
				CreateJwtSecurityKey(appConfig.Auth.Jwt.Secret), 
				SecurityAlgorithms.HmacSha256
			);

			var token = new JwtSecurityToken(
				issuer: appConfig.Auth.Jwt.Issuer,
				audience: audienceSett.Audience.ToLower(),
				claims: claimIdentity.Claims,
				signingCredentials: creds,
				expires: now.AddSeconds(audienceSett.ExpirationSec)
			);

			return (new JwtSecurityTokenHandler()).WriteToken(token);
		}

		/// <summary>
		/// Make a security token
		/// </summary>
		public static string CreateSecurityToken(AppConfig appConfig, JwtAudience audience, JwtArea area, string entityId, string securityStamp, TimeSpan validFor, IEnumerable<Claim> optClaims = null) {

			var now = DateTime.UtcNow;
			var uniqueness = UniqueId(appConfig.Auth.Jwt.Secret);
			var audienceSett = GetAudienceSettings(appConfig, audience);

			var claims = new List<Claim>() {

				// jw main fields
				new Claim(JwtRegisteredClaimNames.Sub, uniqueness),
				new Claim(JwtRegisteredClaimNames.Jti, uniqueness),
				new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
				
				// gm fields
				new Claim(ErIdField, entityId),
				new Claim(ErSecurityStampField, ObtainSecurityStamp(securityStamp)),
				new Claim(ErAreaField, area.ToString().ToLower()),
			};

			if (optClaims != null) {
				claims.AddRange(optClaims);
			}

			var creds = new SigningCredentials(
				CreateJwtSecurityKey(appConfig.Auth.Jwt.Secret), 
				SecurityAlgorithms.HmacSha256
			);

			var token = new JwtSecurityToken(
				issuer: appConfig.Auth.Jwt.Issuer,
				audience: audienceSett.Audience.ToLower(),
				claims: claims,
				signingCredentials: creds,
				expires: now.Add(validFor)
			);

			return (new JwtSecurityTokenHandler()).WriteToken(token);
		}

		///// <summary>
		///// Make a token for Zendesk SSO flow
		///// </summary>
		//public static string CreateZendeskSsoToken(AppConfig appConfig, User user) {
		//	var now = DateTime.UtcNow;
		//	var uniqueness = UniqueId(appConfig.Auth.Jwt.Secret);

		//	var claims = new[] {
		//		new Claim(JwtRegisteredClaimNames.Jti, uniqueness),
		//		new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
		//		new Claim("email", user.NormalizedEmail.ToLower()),
		//		new Claim("name", user.UserName),
		//		new Claim("external_id", user.UserName + "@" + appConfig.Auth.Jwt.Issuer),
		//		new Claim("role", "user"),
		//	};

		//	var creds = new SigningCredentials(
		//		CreateJwtSecurityKey(appConfig.Auth.ZendeskSso.JwtSecret), 
		//		SecurityAlgorithms.HmacSha256
		//	);

		//	var token = new JwtSecurityToken(
		//		issuer: appConfig.Auth.Jwt.Issuer,
		//		claims: claims,
		//		signingCredentials: creds
		//	);

		//	return (new JwtSecurityTokenHandler()).WriteToken(token);
		//}

		// ---

		public static JwtBearerEvents AddEvents() {

			return new JwtBearerEvents() {

				// user must get new token
				OnChallenge = (ctx) => {
					ctx.Response.StatusCode = 403;
					ctx.HandleResponse();
					return Task.CompletedTask;
				},

				OnTokenValidated = async (ctx) => {
					var token = ctx.SecurityToken as JwtSecurityToken;
					try {

						if (token == null) {
							throw new Exception("JWT is null");
						}

						// get passed username and stamp
						var rawUserName = token.Claims.FirstOrDefault((c) => c.Type == ErIdField)?.Value;
						var rawUserStamp = token.Claims.FirstOrDefault((c) => c.Type == ErSecurityStampField)?.Value;
						var rawAudience = token.Audiences.FirstOrDefault();

						if (string.IsNullOrWhiteSpace(rawUserName)) {
							throw new Exception("JWT doesnt contain username");
						}
						if (string.IsNullOrWhiteSpace(rawAudience)) {
							throw new Exception("JWT doesnt contain audience");
						}

						// parse audience
						if (!Enum.TryParse(rawAudience, true, out JwtAudience audience)) {
							throw new Exception("JWT doesnt contain invalid audience");
						}

						// get security stamp of the user
						var dbContext = ctx.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
						var sstamp = (string) null;

						// cabinet
						if (audience == JwtAudience.Cabinet) {
							sstamp = await (
								from u in dbContext.Users
								where u.UserName == rawUserName
								select ObtainSecurityStamp(u.JwtSaltCabinet)
							)
							.AsNoTracking()
							.FirstOrDefaultAsync();
						}
						// dashboard
						else if (audience == JwtAudience.Dashboard) {
							sstamp = await (
								from u in dbContext.Users
								where u.UserName == rawUserName
								select ObtainSecurityStamp(u.JwtSaltDashboard)
							)
							.AsNoTracking()
							.FirstOrDefaultAsync();
						}
						else {
							throw new NotImplementedException($"Audience { rawAudience } is not implemented");
						}

						// compare
						if (sstamp == null || rawUserStamp == null || sstamp != rawUserStamp) {
							throw new Exception("JWT failed to validate sstamp");
						}

						ctx.Success();
					}
					catch (Exception e) {
						ctx.Fail(e);
					}
				}
			};
		}

		public static async Task<bool> IsValid(AppConfig appConfig, string jwtToken, JwtAudience? expectedAudience, JwtArea? expectedArea, Func<JwtSecurityToken, string, Task<string>> validStamp) {
			try {

				// base validation
				JwtSecurityToken token = null;
				{
					new JwtSecurityTokenHandler().ValidateToken(jwtToken, ValidationParameters(appConfig), out var validatedToken);
					token = validatedToken as JwtSecurityToken;
					if (token == null) {
						return false;
					}
				}

				// check id
				var id = token.Claims.FirstOrDefault(_ => _.Type == ErIdField)?.Value;
				if (string.IsNullOrWhiteSpace(id)) {
					return false;
				}

				// check audience
				if (expectedAudience != null) {
					var aud = token.Claims.FirstOrDefault(_ => _.Type == "aud")?.Value;
					if (aud != expectedAudience.ToString().ToLowerInvariant()) {
						return false;
					}
				}

				// check area
				if (expectedArea != null) {
					var area = token.Claims.FirstOrDefault(_ => _.Type == ErAreaField)?.Value;
					if (area != expectedArea.ToString().ToLowerInvariant()) {
						return false;
					}
				}

				// check security stamp
				if (validStamp != null) {
					var valid = await validStamp(token, id);
					if (valid == null) {
						return false;
					}

					var sstamp = token.Claims.FirstOrDefault((c) => c.Type == ErSecurityStampField)?.Value;
					if (sstamp != ObtainSecurityStamp(valid)) {
						return false;
					}
				}

				return true;

			} catch { }

			return false;
		}
	}
}

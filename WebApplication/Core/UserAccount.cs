using Fotron.Common;
using Fotron.Common.Extensions;
using Fotron.DAL;
using Fotron.DAL.Models.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotron.WebApplication.Core {

	public static class UserAccount {

		/// <summary>
		/// New user account
		/// </summary>
		public static async Task<CreateUserAccountResult> CreateUserAccount(IServiceProvider services, string email, string password = null) {

			var logger = services.GetLoggerFor(typeof(UserAccount));
			var dbContext = services.GetRequiredService<ApplicationDbContext>();
			var userManager = services.GetRequiredService<UserManager<User>>();

			var ret = new CreateUserAccountResult() {
			};

			if (string.IsNullOrWhiteSpace(email)) {
				ret.IsEmailExists = true;
				logger.Info("Failed to create user account: invalid email");
				return ret;
			}

			var tfaSecret = GenerateTfaSecret();

			try {

				var newUser = new User() {
					UserName = email,
					Email = email,
					TfaSecret = tfaSecret,
					JwtSaltCabinet = GenerateJwtSalt(),
					JwtSaltDashboard = GenerateJwtSalt(),
					EmailConfirmed = false,
					AccessRights = 0,

					UserOptions = new DAL.Models.UserOptions() {
					},

					TimeRegistered = DateTime.UtcNow,
				};

				var result = (IdentityResult)null;
				if (password != null) {
					result = await userManager.CreateAsync(newUser, password);
				}
				else {
					result = await userManager.CreateAsync(newUser);
				}

				if (result.Succeeded) {
					ret.User = newUser;

					logger.Info($"User account created {newUser.Id}");

					try {
						var name = string.Format("u{0:000000}", newUser.Id);

						newUser.UserName = name;
						newUser.NormalizedUserName = name.ToUpperInvariant();
						newUser.JwtSaltCabinet = GenerateJwtSalt();
						newUser.JwtSaltDashboard = GenerateJwtSalt();
						newUser.AccessRights = (long)AccessRights.Client;

						await dbContext.SaveChangesAsync();

						logger.Info($"User account {newUser.Id} prepared and saved");
					}
					catch { }
				}
				else {
					foreach (var v in result.Errors) {
						if (v.Code == "DuplicateUserName") {
							ret.IsUsernameExists = true;
							logger.Info($"Failed to create user account: duplicate username");
						}
						else if (v.Code == "DuplicateEmail") {
							ret.IsEmailExists = true;
							logger.Info($"Failed to create user account: duplicate email");
						}
						else {
							throw new Exception("Unexpected result error: " + v.Code);
						}
					}
				}
			}
			catch (Exception e) {
				logger?.Error(e, "Failed to create user account");
			}

			return ret;
		}

		/// <summary>
		/// Get proper access rights mask depending on audience and user settings
		/// </summary>
		public static long? ResolveAccessRightsMask(IServiceProvider services, JwtAudience audience, User user) {
			var environment = services.GetRequiredService<IHostingEnvironment>();

			var rights = (long)user.AccessRights;
			var defaultUserMaxRights = 0L | (long)AccessRights.Client;

			if (audience == JwtAudience.Cabinet) {
				// max rights are default user rights
				return user.AccessRights & defaultUserMaxRights;
			}
			else if (audience == JwtAudience.Dashboard) {

				// tfa must be enabled
				if (!user.TwoFactorEnabled) return null;

				// exclude client rights
				rights = (rights - defaultUserMaxRights);

				// has any of dashboard access rights - ok
				if (rights > 0) {
					return rights;
				}
			}
			return null;
		}

		/// <summary>
		/// Random access stamp
		/// </summary>
		private static string GenerateJwtSalt() {
			return SecureRandom.GetString09azAZ(64);
		}

		/// <summary>
		/// Randomize access stamp
		/// </summary>
		public static void GenerateJwtSalt(User user, JwtAudience audience) {
			switch (audience) {
				case JwtAudience.Cabinet: user.JwtSaltCabinet = GenerateJwtSalt(); break;
				case JwtAudience.Dashboard: user.JwtSaltDashboard = GenerateJwtSalt(); break;
				default: throw new NotImplementedException("Audience is not implemented");
			}
		}

		/// <summary>
		/// Current access stamp
		/// </summary>
		public static string CurrentJwtSalt(User user, JwtAudience audience) {
			if (user == null) return null;
			switch (audience) {
				case JwtAudience.Cabinet: return user.JwtSaltCabinet;
				case JwtAudience.Dashboard: return user.JwtSaltDashboard;
				default: throw new NotImplementedException("Audience is not implemented");
			}
		}

		/// <summary>
		/// Random TFA secret
		/// </summary>
		public static string GenerateTfaSecret() {
			return SecureRandom.GetString09azAZSpecs(14);
		}

		// ---

		public sealed class CreateUserAccountResult {

			public User User { get; set; }
			public bool IsEmailExists { get; set; }
			public bool IsUsernameExists { get; set; }
		}

		/// <summary>
		/// Custom user manager class
		/// </summary>
		public class GmUserManager : UserManager<User> {

			public GmUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) {
			}

			public override Task<string> GenerateTwoFactorTokenAsync(User user, string tokenProvider) {
				return Task.FromResult(Tokens.GoogleAuthenticator.Generate(user.TfaSecret));
			}

			public override Task<IList<string>> GetValidTwoFactorProvidersAsync(User user) {
				return Task.FromResult(new List<string>() { "GoogleAuthenticator" } as IList<string>);
			}

			public override Task<bool> VerifyTwoFactorTokenAsync(User user, string tokenProvider, string token) {
				return Task.FromResult(Tokens.GoogleAuthenticator.Validate(token, user.TfaSecret));
			}
		}
	}
}

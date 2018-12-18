using System;
using System.Linq;
using System.Threading.Tasks;
using Fotron.Common;
using Fotron.DAL;
using Fotron.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Fotron.CoreLogic {

	public static class User {

		public static readonly TimeSpan FinancialHistoryOvarlappingOperationsAllowedWithin = TimeSpan.FromMinutes(60);

		public static bool HasFilledPersonalData(UserVerification data) {
			return
				(data?.FirstName?.Length ?? 0) > 0 &&
				(data?.LastName?.Length ?? 0) > 0
			;
		}

		public static bool HasKycVerification(UserVerification data) {
			return
				data?.LastKycTicket != null &&
				data.LastKycTicket.TimeResponded != null &&
				data.LastKycTicket.IsVerified
			;
		}

		public static bool HasTosSigned(UserVerification data) {
			return
				data?.AgreedWithTos != null &&
				data.AgreedWithTos.Value
			;
		}

		public static bool HasProvedResidence(UserVerification data, bool residenceRequried) {
		    if (residenceRequried)
		        return data?.ProvedResidence ?? false;
		    return true;
		}

		// ---

		/// <summary>
		/// User's tier
		/// </summary>
		public static UserTier GetTier(Fotron.DAL.Models.Identity.User user) {
			var tier = UserTier.Tier0;

			var hasPersData = HasFilledPersonalData(user?.UserVerification);
			var hasKyc = HasKycVerification(user?.UserVerification);
			var hasProvedResidence = HasProvedResidence(user?.UserVerification, true);
			var hasAgreement = HasTosSigned(user?.UserVerification);

			if (hasPersData) tier = UserTier.Tier1;

			if (hasKyc && hasProvedResidence && hasAgreement) tier = UserTier.Tier2;

			return tier;
		}

		// ---

		/// <summary>
		/// Persist user activity record
		/// </summary>
		public static UserActivity CreateUserActivity(DAL.Models.Identity.User user, UserActivityType type, string comment, string ip, string agent, Locale locale) {
			return new UserActivity() {
				UserId = user.Id,
				Ip = ip,
				Agent = agent,
				Type = type.ToString().ToLowerInvariant(),
				Comment = comment,
				TimeCreated = DateTime.UtcNow,
				Locale = locale,
			};
		}


		// ---

		public sealed class UpdateUserLimitsData {

			public decimal EthDeposited { get; set; }
			public decimal EthWithdrawn { get; set; }
			public long FiatUsdDeposited { get; set; }
			public long FiatUsdWithdrawn { get; set; }
		}

		/// <summary>
		/// Change user limits
		/// </summary>
		public static async Task UpdateUserLimits(ApplicationDbContext dbContext, long userId, UpdateUserLimitsData data) {

			var nowTime = DateTime.UtcNow;
			var today = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 0, 0, 0, DateTimeKind.Utc);
			var isNew = false;

			// get from db
			var limits = await dbContext.UserLimits.FirstOrDefaultAsync(_ => _.UserId == userId && _.TimeCreated == today);

			// doesn't exist
			if (limits == null) {
				limits = new UserLimits() {
					UserId = userId,
					TimeCreated = today,
				};
				isNew = true;
			}

			limits.EthDeposited += data.EthDeposited;
			limits.EthWithdrawn += data.EthWithdrawn;
			limits.FiatDeposited += data.FiatUsdDeposited;
			limits.FiatWithdrawn += data.FiatUsdWithdrawn;

			if (isNew) {
				await dbContext.UserLimits.AddAsync(limits);
			}

			await dbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Get user limits
		/// </summary>
		public static async Task<UpdateUserLimitsData> GetUserLimits(ApplicationDbContext dbContext, long userId) {

			var limits = await dbContext.UserLimits
				.Where(_ => _.UserId == userId)
				.GroupBy(_ => _.UserId)
				.Select(g => new UpdateUserLimitsData() {
					EthDeposited = g.Sum(_ => _.EthDeposited),
					EthWithdrawn = g.Sum(_ => _.EthWithdrawn),
					FiatUsdDeposited = g.Sum(_ => _.FiatDeposited),
					FiatUsdWithdrawn = g.Sum(_ => _.FiatWithdrawn),
				})
				.FirstOrDefaultAsync();
			;
			if (limits == null) {
				limits = new UpdateUserLimitsData();
			}
			return limits;
		}
	}
}

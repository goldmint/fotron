using System.Net;
using AspNetCore.Totp;

namespace Fotron.WebApplication.Core.Tokens {

	public static class GoogleAuthenticator {

		public const int GenerationPeriodSeconds = 30;

        private static TotpGenerator _totpGenerator { get; set; }

        /// <summary>
        /// Get current user's code
        /// </summary>
        public static string Generate(string secret) {
            _totpGenerator = new TotpGenerator();
			return _totpGenerator.Generate(secret).ToString();
		}

		/// <summary>
		/// Validate code passed by user
		/// </summary>
		public static bool Validate(string code, string secret) {
			bool valid = false;
			int tokeni;
			if (code != null && int.TryParse(code, out tokeni)) {
				var totpv = new TotpValidator(_totpGenerator);
				valid = totpv.Validate(
					secret,
					tokeni,
					GenerationPeriodSeconds
				);
				
			}
			return valid;
		}

		/// <summary>
		/// QR code describes user's access data
		/// </summary>
		public static string MakeQRCode(string issuer, string user, string secretInBase32) {
			return string.Format(
				"otpauth://totp/{1}@{0}?secret={2}&issuer={0}&algorithm=SHA1&period={3}",
				WebUtility.UrlEncode(issuer),
				WebUtility.UrlEncode(user),
				WebUtility.UrlEncode(secretInBase32),
				GenerationPeriodSeconds
			);
		}
	}
}

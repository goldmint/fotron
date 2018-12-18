using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace Fotron.Common {

	public static class ValidationRules {

		public const int PasswordMinLength = 6;
		public const int PasswordMaxLength = 128;

		public static readonly Regex RexUsernameChars = new Regex("^u[0-9]+$");
		public static readonly Regex RexNameChars = new Regex("^[a-zA-Z]{2,32}$");
		public static readonly Regex RexTfaToken = new Regex("^[0-9]{6}$");
		public static readonly Regex RexLatinAndPuncts = new Regex(@"^[a-zA-Z0-9]+[a-zA-Z0-9 \-\,\.\(\)\/]*$");
		public static readonly Regex RexDigits = new Regex(@"^\d+$");
		public static readonly Regex RexTronAddress = new Regex(@"^0x[0-9abcdefABCDEF]{40}$");
		public static readonly Regex RexTronTransactionId = new Regex(@"^0x[0-9abcdefABCDEF]{64}$");

		// ---

		public static bool BeIn(string x, IEnumerable<string> allowed) {
			return allowed.Contains(x);
		}

		public static bool BeValidCaptchaLength(string x) {
			return x != null && x.Trim().Length >= 1;
		}

		public static bool BeValidPhone(string x) {
			if (x == null || !x.StartsWith("+")) {
				return false;
			}

			try {
				var plib = PhoneNumberUtil.GetInstance();
				var numberObj = plib.Parse(x, RegionCode.ZZ);
				return plib.IsValidNumber(numberObj);
			}
			catch {
				return false;
			}
		}

		public static bool BeValidPasswordLength(string x) {
			return x != null && x.Trim().Length >= PasswordMinLength && x.Trim().Length <= PasswordMaxLength;
		}

		public static bool BeValidEmailLength(string x) {
			return x != null && x.Trim().Length >= 5 && x.Trim().Length <= 256;
		}

		public static bool BeValidUsername(string x) {
			return x != null && x.Length > 1 && x.Length <= 64 && RexUsernameChars.IsMatch(x);
		}

		public static bool BeValidId(long x) {
			return x > 0;
		}

		public static bool BeValidConfirmationTokenLength(string x) {
			return x != null && x.Length >= 1;
		}

		public static bool BeValidTfaCode(string x) {
			return x != null && RexTfaToken.IsMatch(x);
		}

		public static bool BeValidName(string x) {
			return x != null && RexNameChars.IsMatch(x);
		}

		public static bool BeValidDate(string x) {
			return x != null && DateTime.TryParseExact(x, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out var _);
		}

		public static bool BeValidCountryCodeAlpha2(string x) {
			if (x != null && x.Length == 2) {
				return Countries.IsValidAlpha2(x);
			}
			return false;
		}

		public static bool ContainLatinAndPuncts(string x) {
			return x != null && RexLatinAndPuncts.IsMatch(x);
		}

		public static bool ContainOnlyDigits(string x) {
			return x != null && RexDigits.IsMatch(x);
		}

		public static bool BeValidUrl(string x) {
			return x != null && Uri.TryCreate(x, UriKind.Absolute, out var test) && (test.Scheme == "http" || test.Scheme == "https");
		}

		public static bool BeValidTronAddress(string x) {
			return x != null && RexTronAddress.IsMatch(x);
		}

		public static bool BeValidTronTransactionId(string x) {
			return x != null && RexTronTransactionId.IsMatch(x);
		}
	}
}

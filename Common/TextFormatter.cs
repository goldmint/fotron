using System;
using System.Numerics;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace Fotron.Common {

	public static class TextFormatter {

		private static readonly Regex _rexReplaceNonDigits = new Regex(@"[^\d]");

		public static string FormatPhoneNumber(string number) {
			number = "+" + _rexReplaceNonDigits.Replace(number, "");

			var plib = PhoneNumberUtil.GetInstance();
			var numberObj = plib.Parse(number, RegionCode.ZZ);

			if (!plib.IsValidNumber(numberObj)) throw new ArgumentException("Phone number is invalid");

			number = plib.Format(numberObj, PhoneNumberFormat.INTERNATIONAL);
			return "+" + _rexReplaceNonDigits.Replace(number, "");
		}

		public static string NormalizePhoneNumber(string number) {
			return "+" + _rexReplaceNonDigits.Replace(FormatPhoneNumber(number), "");
		}

		// ---

		// 1234.40 => 1,234.40
		public static string FormatAmount(long cents) {
			return (cents / 100m).ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
		}

		/// 1512345670000000000, 18 => 1.51234567
		public static string FormatTokenAmount(BigInteger tokenAmount, int tokenDecimals) {
			if (tokenAmount <= 0 || tokenDecimals < 0) return "0";
			var str = tokenAmount.ToString().PadLeft(tokenDecimals + 1, '0');
			str = str.Substring(0, str.Length - tokenDecimals) + "." + str.Substring(str.Length - tokenDecimals);
			return str.TrimEnd('0', '.');
		}
		
		/// 1512345670000000000, 18, 6 => 1.512345
		public static string FormatTokenAmountFixed(BigInteger tokenAmount, int tokenDecimals, int fixDecimals = 6) {
			if (tokenAmount < 0 || tokenDecimals < 0 || fixDecimals < 0 || fixDecimals > tokenDecimals) return "0";
			var str = tokenAmount.ToString().PadLeft(tokenDecimals + 1, '0');
			return str.Substring(0, str.Length - tokenDecimals) + "." + str.Substring(str.Length - tokenDecimals, fixDecimals);
		}

		// 0x0000000000000000000000000000000000000000 => 0x000***000
		public static string MaskBlockchainAddress(string address) {
			if (string.IsNullOrWhiteSpace(address)) return "0x0";
			if (address.Length < 8) return address;
			return address.Substring(0, 5) + "***" + address.Substring(address.Length - 1 - 3, 3);
		}
	}
}

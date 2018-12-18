using System;
using System.Security.Cryptography;
using System.Text;

namespace Fotron.Common {

	public static class SecureRandom {

		private const string DICT_AZ = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string DICT_az = "abcdefghijklmnopqrstuvwxyz";
		private const string DICT_af = "abcdef";
		private const string DICT_09 = "0123456789";
		private const string DICT_Spec = "!@#$%^&*()_-+=[]{}<>~";

		private readonly static char[] PATTERN_09azAZSpecs = (DICT_09 + DICT_az + DICT_AZ + DICT_Spec).ToCharArray();
		private readonly static char[] PATTERN_09azAZ = (DICT_09 + DICT_az + DICT_AZ).ToCharArray();
		private readonly static char[] PATTERN_09az = (DICT_09 + DICT_az).ToCharArray();
		private readonly static char[] PATTERN_09af = (DICT_09 + DICT_af).ToCharArray();
		private readonly static char[] PATTERN_09 = (DICT_09).ToCharArray();
		private readonly static char[] PATTERN_az = (DICT_az).ToCharArray();
		private readonly static char[] PATTERN_AZ = (DICT_AZ).ToCharArray();

		// ---

		public static string GetString(int length, char[] characterSet) {
			if (length < 1 || length > 1024) {
				throw new ArgumentException("length < 1 || length > 1024");
			}
			if (characterSet == null || characterSet.Length == 0) {
				throw new ArgumentException("characterSet == null || characterSet.Length == 0");
			}

			var bytes = new byte[length * 8];
			char[] result = null;
			using (var rnd = new RNGCryptoServiceProvider()) {
				rnd.GetBytes(bytes);
				result = new char[length];
				for (int i = 0; i < length; i++) {
					ulong value = BitConverter.ToUInt64(bytes, i * 8);
					result[i] = characterSet[value % (uint)characterSet.Length];
				}
			}
			if (result == null) {
				throw new Exception("result is null");
			}
			return new string(result);
		}

		public static string GetString09azAZSpecs(int length) {
			return GetString(length, PATTERN_09azAZSpecs);
		}

		public static string GetString09azAZ(int length) {
			return GetString(length, PATTERN_09azAZ);
		}

		public static string GetString09af(int length) {
			return GetString(length, PATTERN_09af);
		}

		public static string GetString09az(int length) {
			return GetString(length, PATTERN_09az);
		}

		public static string GetString09(int length) {
			return GetString(length, PATTERN_09);
		}

		public static string GetStringaz(int length) {
			return GetString(length, PATTERN_az);
		}

		public static string GetStringAZ(int length) {
			return GetString(length, PATTERN_AZ);
		}

		public static string GetStringRandomizeCase(string str) {
			if (string.IsNullOrWhiteSpace(str)) return str;
			var bytes = GetBytes(str.Length);
			if (bytes != null) {
				var sb = new StringBuilder();
				int pos = 0;
				foreach (char c in str) {
					if (bytes[pos++] % 2 == 0) {
						sb.Append(char.ToLower(c));
					}
					else {
						sb.Append(char.ToUpper(c));
					}
				}
				return sb.ToString();
			}
			return str;
		}

		// ---

		/// <summary>
		/// Integer
		/// </summary>
		/// <returns></returns>
		public static int GetInt() {
			try {
				var bytes = new byte[4];
				using (var rnd = new RNGCryptoServiceProvider()) {
					rnd.GetBytes(bytes);
					return BitConverter.ToInt32(bytes, 0);
				}
			}
			catch {
			}
			return Environment.TickCount;
		}

		/// <summary>
		/// Positive integer
		/// </summary>
		public static int GetPositiveInt() {
			var ret = GetInt();
			return ret < 0? -ret: ret;
		}

		/// <summary>
		/// Bytes sequence
		/// </summary>
		public static byte[] GetBytes(int count) {
			if (count <= 0) throw new ArgumentException("count <= 0");

			try {
				var bytes = new byte[count];
				using (var rnd = new RNGCryptoServiceProvider()) {
					rnd.GetBytes(bytes);
					return bytes;
				}
			}
			catch {
			}
			return null;
		}

		/// <summary>
		/// Double: 0..1 both inclusive
		/// </summary>
		public static double GetDouble() {
			try {
				var bytes = new byte[4];
				using (var rnd = new RNGCryptoServiceProvider()) {
					rnd.GetBytes(bytes);
					var val = (double)BitConverter.ToUInt32(bytes, 0);
					return val / 0xFFFFFFFF;
				}
			}
			catch {
			}
			return 0d;
		}
	}
}

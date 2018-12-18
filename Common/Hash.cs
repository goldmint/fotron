namespace Fotron.Common {

	public static class Hash {

		public static string MD5(string input) {

			using (var alg = System.Security.Cryptography.MD5.Create()) {
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
				byte[] hash = alg.ComputeHash(inputBytes);

				var sb = new System.Text.StringBuilder();
				for (int i = 0; i < hash.Length; i++) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString().ToLower();
			}
		}

		public static string SHA1(string input) {

			using (var alg = System.Security.Cryptography.SHA1.Create()) {
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
				byte[] hash = alg.ComputeHash(inputBytes);

				var sb = new System.Text.StringBuilder();
				for (int i = 0; i < hash.Length; i++) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString().ToLower();
			}
		}

		public static string SHA256(string input) {

			using (var alg = System.Security.Cryptography.SHA256.Create()) {
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
				byte[] hash = alg.ComputeHash(inputBytes);

				var sb = new System.Text.StringBuilder();
				for (int i = 0; i < hash.Length; i++) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString().ToLower();
			}
		}

		public static string SHA512(string input) {

			using (var alg = System.Security.Cryptography.SHA512.Create()) {
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
				byte[] hash = alg.ComputeHash(inputBytes);

				var sb = new System.Text.StringBuilder();
				for (int i = 0; i < hash.Length; i++) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString().ToLower();
			}
		}

		public static string HMACSHA256(string input, string key) {
			var keyBytes = System.Text.Encoding.ASCII.GetBytes(key);
			using (var alg = new System.Security.Cryptography.HMACSHA256(keyBytes)) {
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
				byte[] hash = alg.ComputeHash(inputBytes);

				var sb = new System.Text.StringBuilder();
				for (int i = 0; i < hash.Length; i++) {
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString().ToLower();
			}
		}
	}
}

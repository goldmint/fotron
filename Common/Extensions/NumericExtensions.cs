using System.Numerics;

namespace Fotron.Common.Extensions {

	public static class NumericExtensions {

		public static decimal FromWei(this BigInteger v) {
			return (decimal) v / (decimal)BigInteger.Pow(10, TokensPrecision.Tron);
		}

		public static BigInteger ToEther(this decimal v) {
			return new BigInteger(decimal.Floor(v * (decimal) BigInteger.Pow(10, TokensPrecision.Tron)));
		}
	}
}

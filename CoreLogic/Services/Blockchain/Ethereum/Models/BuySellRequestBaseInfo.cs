using System.Numerics;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Models {

	public sealed class BuySellRequestBaseInfo {

		/// <summary>
		/// User address
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Request input amount
		/// </summary>
		public BigInteger InputAmount { get; set; }

		/// <summary>
		/// Request output amount
		/// </summary>
		public BigInteger OutputAmount { get; set; }

		/// <summary>
		/// Is pending
		/// </summary>
		public bool IsPending { get; set; }

		/// <summary>
		/// Is succeeded
		/// </summary>
		public bool IsSucceeded { get; set; }

		/// <summary>
		/// Is cancelled
		/// </summary>
		public bool IsCancelled { get; set; }
		
		/// <summary>
		/// Is failed
		/// </summary>
		public bool IsFailed { get; set; }
	}
}

using System.Numerics;

namespace Fotron.CoreLogic.Services.Blockchain.Tron.Models.ContractEvent {

	public sealed class TokenSellRequest {

		/// <summary>
		/// User address
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// User ID
		/// </summary>
		public string UserId { get; set; }
		
		/// <summary>
		/// Reference / internal request ID
		/// </summary>
		public BigInteger Reference { get; set; }

		/// <summary>
		/// Amount (input amount, GOLD)
		/// </summary>
		public BigInteger Amount { get; set; }

		/// <summary>
		/// Request index at contract storage
		/// </summary>
		public BigInteger RequestIndex { get; set; }

		// ---

		/// <summary>
		/// Block number
		/// </summary>
		public BigInteger BlockNumber { get; set; }

		/// <summary>
		/// Transaction ID
		/// </summary>
		public string TransactionId { get; set; }
	}
}

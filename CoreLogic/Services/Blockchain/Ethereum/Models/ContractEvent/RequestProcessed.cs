using System.Numerics;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Models.ContractEvent {

	public sealed class RequestProcessed {

		/// <summary>
		/// Request index at contract storage
		/// </summary>
		public BigInteger RequestIndex { get; set; }

		/// <summary>
		/// Is buying/selling request
		/// </summary>
		public bool IsBuyRequest { get; set; }

		/// <summary>
		/// Is eth/fiat request
		/// </summary>
		public bool IsFiat { get; set; }

		/// <summary>
		/// Request output amount in GOLD/ETH/fiat
		/// </summary>
		public BigInteger Amount { get; set; }

		/// <summary>
		/// Rate of GOLD/ETH depending on request type
		/// </summary>
		public BigInteger Rate { get; set; }

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

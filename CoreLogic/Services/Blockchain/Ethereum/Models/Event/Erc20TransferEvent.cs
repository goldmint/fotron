using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Models.Event {

	public class Erc20TransferEvent : BaseEvent {

		public Erc20TransferEvent(BigInteger block, string hash, Erc20TransferEventMapping map) {
			BlockNumber = block;
			Transaction = hash;

			From = map.From;
			To = map.To;
			Amount = map.Value;
		}

		/// <summary>
		/// From address
		/// </summary>
		public string From { get; set; }

		/// <summary>
		/// To address
		/// </summary>
		public string To { get; set; }

		/// <summary>
		/// Amount
		/// </summary>
		public BigInteger Amount { get; set; }
	}

	public class Erc20TransferEventMapping {

		[Parameter("address", "_from", 1, true)]
		public string From { get; set; }

		[Parameter("address", "_to", 2, true)]
		public string To { get; set; }

		[Parameter("uint", "_value", 3, false)]
		public BigInteger Value { get; set; }
	}

}

using Fotron.Common;
using System;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Models {

	public sealed class TransactionInfo {

		public EthTransactionStatus Status { get; internal set; }
		public DateTime? Time { get; internal set; }
	}
}

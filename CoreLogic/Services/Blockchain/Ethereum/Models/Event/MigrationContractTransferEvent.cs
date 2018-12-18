using System.Numerics;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Models.Event {

	public class MigrationContractTransferEvent : Erc20TransferEvent {

		/// <summary>
		/// Token contract address
		/// </summary>
		public string TokenContractAddress { get; set; }

		// ---

		public MigrationContractTransferEvent(string token, BigInteger block, string hash, Erc20TransferEventMapping map) : base(block, hash, map) {
			TokenContractAddress = token;
		}
	}

}

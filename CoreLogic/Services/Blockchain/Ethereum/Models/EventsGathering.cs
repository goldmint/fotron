using System.Numerics;
using Fotron.CoreLogic.Services.Blockchain.Tron.Models.ContractEvent;

namespace Fotron.CoreLogic.Services.Blockchain.Tron.Models {

	public sealed class GatheredTokenBuyEvents {

		public BigInteger FromBlock { get; set; }
		public BigInteger ToBlock { get; set; }
		public TokenBuyRequest[] Events { get; set; }
	}

	public sealed class GatheredTokenSellEvents {

		public BigInteger FromBlock { get; set; }
		public BigInteger ToBlock { get; set; }
		public TokenSellRequest[] Events { get; set; }
	}

	public sealed class GatheredRequestProcessedEvents {

		public BigInteger FromBlock { get; set; }
		public BigInteger ToBlock { get; set; }
		public RequestProcessed[] Events { get; set; }
	}

}

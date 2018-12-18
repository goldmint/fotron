using System.Numerics;
using Fotron.CoreLogic.Services.Blockchain.Ethereum.Models.Event;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Models {

	public class GatheredLog<T> where T : BaseEvent {

		public BigInteger FromBlock { get; set; }
		public BigInteger ToBlock { get; set; }
		public T[] Events { get; set; }
	}
}

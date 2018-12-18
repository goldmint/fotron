using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Models.Event {

	public abstract class BaseEvent {

		/// <summary>
		/// Block number
		/// </summary>
		public BigInteger BlockNumber { get; set; }

		/// <summary>
		/// Transaction ID
		/// </summary>
		public string Transaction { get; set; }
	}
}

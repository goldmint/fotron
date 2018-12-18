using Fotron.Common;
using System.Numerics;
using System.Threading.Tasks;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum {

	public interface IEthereumWriter
	{
	    Task<string> UpdateMaxGaxPrice(BigInteger gasPrice);
	}
}

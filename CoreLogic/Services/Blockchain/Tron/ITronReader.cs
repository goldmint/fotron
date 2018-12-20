using System.Numerics;
using System.Threading.Tasks;

namespace Fotron.CoreLogic.Services.Blockchain.Tron {

	public interface ITronReader {

		Task<long> GetLatestBlockNumber();
		//Task<BigInteger> GetCurrentGasPrice();

	    Task<decimal> GetTokenPrice(string contactAddress);
	    Task<long> GetBuyCount(string contactAddress);
	    Task<long> GetSellCount(string contactAddress);
	    Task<decimal> GetBonusPerShare(string contactAddress);
	    Task<decimal> GetVolumeEth(string contactAddress);
	    Task<decimal> GetVolumeToken(string contactAddress);
    }
}

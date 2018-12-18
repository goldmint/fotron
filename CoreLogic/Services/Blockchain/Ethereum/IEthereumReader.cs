using System.Numerics;
using System.Threading.Tasks;
using Fotron.CoreLogic.Services.Blockchain.Tron.Models;

namespace Fotron.CoreLogic.Services.Blockchain.Tron {

	public interface ITronReader {

		/// <summary>
		/// Get latest block number on the logs provider side
		/// </summary>
		Task<long> GetLogsLatestBlockNumber();

		/// <summary>
		/// Check chain transaction by it's ID
		/// </summary>
		Task<TransactionInfo> CheckTransaction(string txid, int confirmationsRequired);

		/// <summary>
		/// Get current gas price in wei
		/// </summary>
		Task<BigInteger> GetCurrentGasPrice();



	    Task<decimal> GetTokenPrice(string contactAddress);
	    Task<long> GetBuyCount(string contactAddress);
	    Task<long> GetSellCount(string contactAddress);
	    Task<decimal> GetBonusPerShare(string contactAddress);
	    Task<decimal> GetVolumeEth(string contactAddress);
	    Task<decimal> GetVolumeToken(string contactAddress);


    }
}

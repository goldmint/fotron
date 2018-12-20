using Fotron.Common;
using NLog;

namespace Fotron.CoreLogic.Services.Blockchain.Tron.Impl {

	public sealed class TronWriter : TronBaseClient, ITronWriter {


		public TronWriter(AppConfig appConfig, LogFactory logFactory) : base(appConfig, logFactory) {
		}


		//public async Task<string> UpdateMaxGaxPrice(BigInteger gasPrice) {
		//	return await Web3Utils.SendTransaction(AppConfig.Services.Tron.FotronCoreAddress,
		//		AppConfig.Services.Tron.FotronCoreAbi, AppConfig.Services.Tron.SetMaxGasPriceFunctionName,
		//		AppConfig.Services.Tron.ManagerPrivateKey, 100000, gasPrice, 0, gasPrice);
		//}
	}
}

using Fotron.Common;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using NLog;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Fotron.CoreLogic.Services.Blockchain.Tron.Impl {

	public sealed class TronWriter : TronBaseClient, ITronWriter {


		public TronWriter(AppConfig appConfig, LogFactory logFactory) : base(appConfig, logFactory) {
		}


	    public async Task<string> UpdateMaxGaxPrice(BigInteger gasPrice)
	    {
	        return await Web3Utils.SendTransaction(AppConfig.Services.Tron.FotronCoreAddress,
                AppConfig.Services.Tron.FotronCoreAbi, AppConfig.Services.Tron.SetMaxGasPriceFunctionName,
                AppConfig.Services.Tron.ManagerPrivateKey, 100000, gasPrice, 0, gasPrice);
	    }
	}
}

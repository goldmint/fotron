using Fotron.Common;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using NLog;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Impl {

	public sealed class EthereumWriter : EthereumBaseClient, IEthereumWriter {


		public EthereumWriter(AppConfig appConfig, LogFactory logFactory) : base(appConfig, logFactory) {
		}


	    public async Task<string> UpdateMaxGaxPrice(BigInteger gasPrice)
	    {
	        return await Web3Utils.SendTransaction(AppConfig.Services.Ethereum.FotronCoreAddress,
                AppConfig.Services.Ethereum.FotronCoreAbi, AppConfig.Services.Ethereum.SetMaxGasPriceFunctionName,
                AppConfig.Services.Ethereum.ManagerPrivateKey, 100000, gasPrice, 0, gasPrice);
	    }
	}
}

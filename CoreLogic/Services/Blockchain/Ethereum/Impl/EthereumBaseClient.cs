using Fotron.Common;
using NLog;
using System;
using Fotron.Common.Extensions;

namespace Fotron.CoreLogic.Services.Blockchain.Ethereum.Impl {

	public abstract class EthereumBaseClient {

		protected ILogger Logger { get; }
		protected Nethereum.JsonRpc.Client.IClient EthProvider { get; }
		protected Nethereum.JsonRpc.Client.IClient EthLogsProvider { get; }
		protected AppConfig AppConfig { get; }
		protected Web3Utils Web3Utils { get; }


		// ---

		protected EthereumBaseClient(AppConfig appConfig, LogFactory logFactory) {
			Logger = logFactory.GetLoggerFor(this);
			AppConfig = appConfig;

			EthProvider = new Nethereum.JsonRpc.Client.RpcClient(new Uri(appConfig.Services.Ethereum.Provider));
			//EthLogsProvider = new Nethereum.JsonRpc.Client.RpcClient(new Uri(appConfig.Services.Ethereum.LogsProvider));

			Web3Utils = new Web3Utils(EthProvider);
		}
	}
}

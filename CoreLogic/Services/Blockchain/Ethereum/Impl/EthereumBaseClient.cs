using Fotron.Common;
using NLog;
using System;
using Fotron.Common.Extensions;

namespace Fotron.CoreLogic.Services.Blockchain.Tron.Impl {

	public abstract class TronBaseClient {

		protected ILogger Logger { get; }
		protected Nethereum.JsonRpc.Client.IClient EthProvider { get; }
		protected Nethereum.JsonRpc.Client.IClient EthLogsProvider { get; }
		protected AppConfig AppConfig { get; }
		protected Web3Utils Web3Utils { get; }


		// ---

		protected TronBaseClient(AppConfig appConfig, LogFactory logFactory) {
			Logger = logFactory.GetLoggerFor(this);
			AppConfig = appConfig;

			EthProvider = new Nethereum.JsonRpc.Client.RpcClient(new Uri(appConfig.Services.Tron.Provider));
			//EthLogsProvider = new Nethereum.JsonRpc.Client.RpcClient(new Uri(appConfig.Services.Tron.LogsProvider));

			Web3Utils = new Web3Utils(EthProvider);
		}
	}
}

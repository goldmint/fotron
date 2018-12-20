using Fotron.Common;
using Fotron.Common.Extensions;
using NLog;

namespace Fotron.CoreLogic.Services.Blockchain.Tron.Impl {

	public abstract class TronBaseClient {

		protected ILogger Logger { get; }
		protected AppConfig AppConfig { get; }
		protected string API { get; }

		// ---

		protected TronBaseClient(AppConfig appConfig, LogFactory logFactory) {
			Logger = logFactory.GetLoggerFor(this);
			AppConfig = appConfig;
			API = appConfig.Services.Tron.Provider;
		}
	}
}

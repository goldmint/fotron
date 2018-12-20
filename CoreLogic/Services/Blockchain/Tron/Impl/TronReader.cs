using Fotron.Common;
using Fotron.Common.Extensions;
using NLog;
using System;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Fotron.Common.WebRequest;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Utilities.Encoders;

namespace Fotron.CoreLogic.Services.Blockchain.Tron.Impl {

	public class TronReader : TronBaseClient, ITronReader {

		public TronReader(AppConfig appConfig, LogFactory logFactory) : base(appConfig, logFactory) {
		}

		// ---

		//public async Task<BigInteger> GetCurrentGasPrice() {
		//	var obj = await Utils.DownloadDynamicObj(AppConfig.Services.Tron.GasPriceUrl);

		//	decimal gasPrice = obj?.fast ?? 20;

		//	return Web3Utils.Convert.ToWei(gasPrice, UnitConversion.EthUnit.Gwei);
		//}

		public async Task<long> GetLatestBlockNumber() {
			return (long) await getLatestBlockNumber();
		}

		public async Task<decimal> GetTokenPrice(string contactAddress) {
			var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.TokenPriceFunctionName);
			return val.FromWei();
		}

		public async Task<long> GetBuyCount(string contactAddress) {
			var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.TokenBuyCountFunctionName);
			return (long)val;
		}

		public async Task<long> GetSellCount(string contactAddress) {
			var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.TokenSellCountFunctionName);
			return (long)val;
		}

		public async Task<decimal> GetBonusPerShare(string contactAddress) {
			var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.BonusPerShareFunctionName);
			return val.FromWei();
		}

		public async Task<decimal> GetVolumeEth(string contactAddress) {
			var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.VolumeEthFunctionName);
			return val.FromWei();
		}

		public async Task<decimal> GetVolumeToken(string contactAddress) {
			var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.VolumeTokenFunctionName);
			return val.FromWei();
		}

		private async Task<BigInteger> GetViewFuntionResult(string contactAddress, string functionName) {
			var res = await callContractConstantMethod(contactAddress, functionName, null);
			if (res == null || res.Length != 1) {
				throw new Exception("Invalid result length");
			}
			return BigInteger.Parse(res[0], NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
		}

		// ---

		protected async Task<string[]> callContractConstantMethod(string contractAddress, string methodSelector, string[] paramsHex) {

			var addressHex = (string)null;
			try {
				Hex.Decode(contractAddress);
				addressHex = contractAddress;
			}
			catch {
				try {
					addressHex = Hex.ToHexString(Base58CheckEncoding.Decode(contractAddress));
				}
				catch {
					throw new ArgumentException("Invalid contract address");
				}
			}

			var parameter = (string)null;
			if (paramsHex != null && paramsHex.Length > 0) {
				parameter = paramsHex.Join("");
			}

			var body = new {
				contract_address = addressHex,
				function_selector = methodSelector,
				parameter = parameter,
			};

			var result = new ContractConstantMethodResult() {
				ConstantResult = null,
				Result = new ContractConstantMethodResult.ResultData() {
					Result = false,
				}
			};

			using (var req = new Request(Logger)) {
				await req
						.AcceptJson()
						.BodyJson(body)
						.OnResult(async (res) => {
							if (res.GetHttpStatus() == null || res.GetHttpStatus().Value != HttpStatusCode.OK) {
								throw new Exception("Invalid HTTP status");
							}
							var raw = await res.ToRawString();
							if (!Json.ParseInto(raw, result, Json.SnakeCaseSettings)) {
								throw new Exception("Failed to parse");
							}
						})
						.SendPost(string.Concat(API.TrimEnd('/'), "/wallet/triggersmartcontract"))
					;
			}

			if (!(result?.Result?.Result ?? false) || (result?.ConstantResult?.Length ?? 0) == 0) {
				throw new Exception("Invalid result");
			}

			return result.ConstantResult;
		}

		internal class ContractConstantMethodResult {
			public string[] ConstantResult { get;set; }
			public ResultData Result { get;set; }
			public class ResultData {
				public bool Result { get; set; }
			}
		}

		// ---

		protected async Task<ulong> getLatestBlockNumber() {

			var result = new LatestBlockResult {
				BlockHeader = new LatestBlockResult.Header() {
					RawData = new LatestBlockResult.Header.Raw() {
						Number = 0L,
					}
				}
			};

			using (var req = new Request(Logger)) {
				await req
						.AcceptJson()
						.OnResult(async (res) => {
							if (res.GetHttpStatus() == null || res.GetHttpStatus().Value != HttpStatusCode.OK) {
								throw new Exception("Invalid HTTP status");
							}
							var raw = await res.ToRawString();
							if (!Json.ParseInto(raw, result, Json.SnakeCaseSettings)) {
								throw new Exception("Failed to parse");
							}
						})
						.SendGet(string.Concat(API.TrimEnd('/'), "/wallet/getnowblock"))
					;
			}
			
			return result?.BlockHeader?.RawData?.Number ?? 0L;
		}

		internal class LatestBlockResult {
			public Header BlockHeader { get;set; }
			public class Header {
				public Raw RawData { get; set; }
				public class Raw {
					public ulong Number { get; set; }
				}
			}
		}
	}
}

using Fotron.Common;
using Nethereum.Web3;
using NLog;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Fotron.Common.Extensions;
using Fotron.CoreLogic.Services.Blockchain.Tron.Models;
using Nethereum.Util;

namespace Fotron.CoreLogic.Services.Blockchain.Tron.Impl
{

    public class TronReader : TronBaseClient, ITronReader
    {

        public TronReader(AppConfig appConfig, LogFactory logFactory) : base(appConfig, logFactory)
        {
        }

        // ---

        public async Task<BigInteger> GetCurrentGasPrice()
        {
            var obj = await Utils.DownloadDynamicObj(AppConfig.Services.Tron.GasPriceUrl);

            decimal gasPrice = obj?.fast ?? 20;

            return Web3Utils.Convert.ToWei(gasPrice, UnitConversion.EthUnit.Gwei);
        }

        public async Task<long> GetLogsLatestBlockNumber()
        {
            var syncResp = await Web3Utils.Web3.Eth.Syncing.SendRequestAsync();

            if (syncResp.IsSyncing) return (long) syncResp.CurrentBlock.Value;

            return (long) (await Web3Utils.Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
        }

        public async Task<TransactionInfo> CheckTransaction(string txid, int confirmationsRequired)
        {

            if (string.IsNullOrWhiteSpace(txid))
            {
                throw new ArgumentException("Invalid transaction format");
            }

            var web3 = new Web3(EthProvider);
            var txinfo = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txid);

            if (txinfo == null) return new TransactionInfo { Status = EthTransactionStatus.Pending };


            var lastBlockNum = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            var threshold = BigInteger.One * Math.Max(2, confirmationsRequired);

            if (
                txinfo.BlockNumber.HexValue != null && // got into block
                lastBlockNum.Value - txinfo.BlockNumber.Value >= threshold // wait for number of confirmation
            )
            {
                var txBlockInfo = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(txinfo.BlockNumber);
                var blockTimestamp = (DateTime?)null;
                if (txBlockInfo?.Timestamp != null && txBlockInfo.Timestamp.Value > 0)
                {
                    blockTimestamp = DateTimeOffset.FromUnixTimeSeconds((long)txBlockInfo.Timestamp.Value).UtcDateTime;
                }

                // check status
                if ((txinfo.Status?.Value ?? BigInteger.Zero) == BigInteger.One)
                {
                    return new TransactionInfo()
                    {
                        Status = EthTransactionStatus.Success,
                        Time = blockTimestamp,
                    };
                }
                return new TransactionInfo()
                {
                    Status = EthTransactionStatus.Failed,
                    Time = blockTimestamp,
                };
            }
            return new TransactionInfo()
            {
                Status = EthTransactionStatus.Pending,
            };

            // assume it is pending
        }


        public async Task<decimal> GetTokenPrice(string contactAddress)
        {
            var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.TokenPriceFunctionName);

            return val.FromWei();
        }

        public async Task<long> GetBuyCount(string contactAddress)
        {
            var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.TokenBuyCountFunctionName);

            return (long)val;
        }

        public async Task<long> GetSellCount(string contactAddress)
        {
            var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.TokenSellCountFunctionName);

            return (long)val;
        }

        public async Task<decimal> GetBonusPerShare(string contactAddress)
        {
            var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.BonusPerShareFunctionName);

            return val.FromWei();
        }

        public async Task<decimal> GetVolumeEth(string contactAddress)
        {
            var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.VolumeEthFunctionName);

            return val.FromWei();
        }

        public async Task<decimal> GetVolumeToken(string contactAddress)
        {
            var val = await GetViewFuntionResult(contactAddress, AppConfig.Services.Tron.VolumeTokenFunctionName);

            return val.FromWei();
        }


        private async Task<BigInteger> GetViewFuntionResult(string contactAddress, string functionName)
        {
            return await Web3Utils.GetViewFunctionResult<BigInteger>(contactAddress, AppConfig.Services.Tron.FotronContractAbi, functionName);
        }
    }
}

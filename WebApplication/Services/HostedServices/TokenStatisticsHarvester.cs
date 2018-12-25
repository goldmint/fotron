using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fotron.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Fotron.WebApplication.Services.HostedServices {

	public class TokenStatisticsHarvester : BaseHostedService {

		protected override TimeSpan Period => TimeSpan.FromMinutes(5);

		public TokenStatisticsHarvester(IServiceProvider services) : base(services) { }

		protected override async Task OnInit() {
			await base.OnInit();
		}

		protected override async Task DoWork() {
			
			DbContext.DetachEverything();
			var tokenList = await DbContext.Tokens.Where(x => x.IsEnabled && !x.IsDeleted).ToListAsync();
			var thisDayStart = DayStartOf(DateTime.Now);

			foreach (var token in tokenList) {

				var lastStat = await DbContext.TokenStatistics.LastOrDefaultAsync(x => x.TokenId == token.Id);
				var add = lastStat == null || lastStat.Date != thisDayStart;

				var price = await TronObserver.GetTokenPrice(token.FotronContractAddress);
				var buyCount = await TronObserver.GetBuyCount(token.FotronContractAddress);
				var sellCount = await TronObserver.GetSellCount(token.FotronContractAddress);
				var bonusPerShare = await TronObserver.GetBonusPerShare(token.FotronContractAddress);
				var volumeEth = await TronObserver.GetVolumeEth(token.FotronContractAddress);
				var volumeToken = await TronObserver.GetVolumeToken(token.FotronContractAddress);
				var blockNum = await TronObserver.GetLatestBlockNumber();

				if (add) {
					var tokenStat = new TokenStatistics {
						Date = thisDayStart,
						PriceEth = price,
						BuyCount = buyCount,
						SellCount = sellCount,
						ShareReward = bonusPerShare,
						VolumeEth = volumeEth,
						VolumeToken = volumeToken,
						BlockNum = blockNum,
						TokenId = token.Id
					};
					await DbContext.TokenStatistics.AddAsync(tokenStat);
				}
				else {
					lastStat.PriceEth = price;
					lastStat.BuyCount = buyCount;
					lastStat.SellCount = sellCount;
					lastStat.ShareReward = bonusPerShare;
					lastStat.VolumeEth = volumeEth;
					lastStat.VolumeToken = volumeToken;
					lastStat.BlockNum = blockNum;
				}
			}

			await DbContext.SaveChangesAsync();
		}

		private DateTime DayStartOf(DateTime t) {
			return new DateTime(t.Year, t.Month, t.Day, 0, 0, 0, t.Kind);
		}
	}
}

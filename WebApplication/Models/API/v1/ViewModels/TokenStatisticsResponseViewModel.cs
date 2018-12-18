using System;

namespace Fotron.WebApplication.Models.API.v1.ViewModels
{
    public class TokenStatisticsResponseViewModel
    {
        public long TokenId { get; set; }

        public DateTime Date { get; set; }

        public decimal PriceEth { get; set; }

        public long BuyCount { get; set; }

        public long SellCount { get; set; }

        public decimal ShareReward { get; set; }

        public long TotalTxCount => BuyCount + SellCount;
    }
}

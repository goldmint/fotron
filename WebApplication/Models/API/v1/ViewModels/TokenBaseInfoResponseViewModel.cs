using System.Collections.Generic;

namespace Fotron.WebApplication.Models.API.v1.ViewModels
{
    public class TokenBaseInfoResponseViewModel
    {
        public long Id { get; set; }

        public string TokenContractAddress { get; set; }

        public string FotronContractAddress { get; set; }

        public string FullName { get; set; }

        public string Ticker { get; set; }

        public string LogoUrl { get; set; }

        public decimal StartPriceEth { get; set; }

        public decimal CurrentPriceEth { get; set; }

        public decimal PriceChangeLastDayPercent { get; set; }

        public decimal TradingVolume24HEth { get; set; }

        public List<decimal> PriceStatistics7D { get; set; }
    }
}

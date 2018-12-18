using System.ComponentModel.DataAnnotations;

namespace Fotron.WebApplication.Models.API.v1.ViewModels
{
    public class AddTokenRequestViewModel
    {
        [Required]
        public string CompanyName { get; set; }

        public string WebsiteUrl { get; set; }

        [Required]
        public string ContactEmail { get; set; }

        [Required]
        public string TokenTicker { get; set; }

        public string TokenContractAddress { get; set; }

        [Required]
        public decimal StartPriceEth { get; set; }

        [Required]
        public long TotalSupply { get; set; }
    }
}

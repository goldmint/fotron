using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fotron.DAL.Models.Base;

namespace Fotron.DAL.Models
{
    [Table("ft_add_token_request")]
    public class AddTokenRequest : DbBaseEntity
    {
        [Column("company_name"), Required, MaxLength(128)]
        public string CompanyName { get; set; }

        [Column("website_url"), Required, MaxLength(128)]
        public string WebsiteUrl { get; set; }

        [Column("contact_email"), Required, MaxLength(128)]
        public string ContactEmail { get; set; }


        [Column("token_ticker"), Required, MaxLength(128)]
        public string TokenTicker { get; set; }

        [Column("token_contract_address"), MaxLength(43)]
        public string TokenContractAddress { get; set; }
        
        [Column("start_price_eth")]
        public decimal StartPriceEth { get; set; }

        [Column("total_supply")]
        public long TotalSupply { get; set; }

        [Column("time_created"), Required]
        public DateTime TimeCreated { get; set; }
    }
}

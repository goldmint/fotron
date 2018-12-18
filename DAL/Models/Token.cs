using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fotron.DAL.Models.Base;

namespace Fotron.DAL.Models
{
    [Table("er_token")]
    public class Token : DbBaseEntity
    {
        [Column("token_contract_address"), Required, MaxLength(43)]
        public string TokenContractAddress { get; set; }

        [Column("fotron_contract_address"), Required, MaxLength(43)]
        public string FotronContractAddress { get; set; }

        [Column("full_name"), Required, MaxLength(128)]
        public string FullName { get; set; }

        [Column("ticker"), Required, MaxLength(16)]
        public string Ticker { get; set; }

        [Column("description"), Required, MaxLength(1024)]
        public string Description { get; set; }

        [Column("logo_url"), Required, MaxLength(1024)]
        public string LogoUrl { get; set; }

        [Column("start_price_eth"), Required, MaxLength(1024)]
        public decimal StartPriceEth { get; set; }

        [Column("current_price_eth"), Required, MaxLength(1024)]
        public decimal CurrentPriceEth { get; set; }

        [Column("time_created"), Required]
        public DateTime TimeCreated { get; set; }

        [Column("time_updated"), Required]
        public DateTime TimeUpdated { get; set; }
    }
}

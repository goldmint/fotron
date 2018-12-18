using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fotron.DAL.Models.Base;

namespace Fotron.DAL.Models
{
    [Table("er_token_statistics")]
    public class TokenStatistics : DbBaseEntity
    {
        [Column("token_id"), Required]
        public long TokenId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("price_eth")]
        public decimal PriceEth { get; set; }

        [Column("buy_count")]
        public long BuyCount { get; set; }

        [Column("sell_count")]
        public long SellCount { get; set; }

        [Column("share_reward")]
        public decimal ShareReward { get; set; }

        [Column("volume_eth")]
        public decimal VolumeEth { get; set; }

        [Column("volume_token")]
        public decimal VolumeToken { get; set; }

        [Column("block_num")]
        public long BlockNum { get; set; }

        [ForeignKey(nameof(TokenId))]
        public virtual Token Token { get; set; }
    }
}

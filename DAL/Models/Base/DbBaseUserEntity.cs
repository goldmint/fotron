using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fotron.DAL.Models.Identity;

namespace Fotron.DAL.Models.Base
{
    public abstract class DbBaseUserEntity : DbBaseEntity
    {
        [Column("user_id"), Required]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}

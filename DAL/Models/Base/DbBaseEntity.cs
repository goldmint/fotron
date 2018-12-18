using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fotron.DAL.Models.Base {

    public abstract class DbBaseEntity {

        [Key, Column("id")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

        [Column("is_enabled"), DefaultValue(true)]
        public bool IsEnabled { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

    }
}

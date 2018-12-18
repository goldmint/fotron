using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fotron.DAL.Models.Base;

namespace Fotron.DAL.Models {

	[Table("ft_settings")]
	public class Settings : DbBaseEntity, IConcurrentUpdate {

		[Column("key"), MaxLength(MaxKeyFieldLength), Required]
		public string Key { get; set; }

		[Column("value"), MaxLength(MaxValueFieldLength)]
		public string Value { get; set; }

		[Column("concurrency_stamp"), MaxLength(64), ConcurrencyCheck]
		public string ConcurrencyStamp { get; set; }

		// ---

		public const int MaxKeyFieldLength = 64;
		public const int MaxValueFieldLength = 16384;

		public void OnConcurrencyStampRegen() {
			this.ConcurrencyStamp = ConcurrentStamp.GetGuid();
		}
	}
}

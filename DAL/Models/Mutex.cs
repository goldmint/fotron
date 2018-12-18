using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fotron.DAL.Models {

	[Table("er_mutex")]
	public class Mutex {

		[Key, Column("id"), MaxLength(64), Required]
		public string Id { get; set; }

		[Column("locker"), MaxLength(32), Required, ConcurrencyCheck]
		public string Locker { get; set; }

		[Column("expires"), Required, ConcurrencyCheck]
		public DateTime Expires { get; set; }
	}
}

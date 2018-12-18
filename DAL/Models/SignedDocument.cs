using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fotron.Common;
using Fotron.DAL.Models.Base;

namespace Fotron.DAL.Models {

	[Table("ft_signed_document")]
	public class SignedDocument : DbBaseUserEntity {

		[Column("type"), Required]
		public SignedDocumentType Type { get; set; }

		[Column("is_signed"), Required]
		public bool IsSigned { get; set; }

		[Column("reference_id"), Required, MaxLength(64)]
		public string ReferenceId { get; set; }

		[Column("callback_status"), MaxLength(16)]
		public string CallbackStatus { get; set; }

		[Column("callback_event_type"), MaxLength(64)]
		public string CallbackEvent { get; set; }

		[Column("secret"), Required, MaxLength(64)]
		public string Secret { get; set; }

		[Column("time_created"), Required]
		public DateTime TimeCreated { get; set; }

		[Column("time_completed")]
		public DateTime? TimeCompleted { get; set; }
	}
}

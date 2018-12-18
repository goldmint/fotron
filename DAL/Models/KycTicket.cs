using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fotron.DAL.Models.Base;

namespace Fotron.DAL.Models {

	[Table("er_kyc_shuftipro_ticket")]
	public class KycTicket : DbBaseUserEntity {

		[Column("reference_id"), Required, MaxLength(32)]
		public string ReferenceId { get; set; }

		[Column("is_verified"), Required]
		public bool IsVerified { get; set; }

		[Column("callback_status_code"), MaxLength(32)]
		public string CallbackStatusCode { get; set; }

		[Column("callback_message"), MaxLength(128)]
		public string CallbackMessage { get; set; }

		[Column("method"), Required, MaxLength(32)]
		public string Method { get; set; }

		[Column("first_name"), Required, MaxLength(64)]
		public string FirstName { get; set; }

		[Column("last_name"), Required, MaxLength(64)]
		public string LastName { get; set; }

		[Column("country_code"), Required, MaxLength(2)]
		public string CountryCode { get; set; }

		[Column("dob"), Required]
		public DateTime DoB { get; set; }

		[Column("phone_number"), Required, MaxLength(32)]
		public string PhoneNumber { get; set; }

		[Column("time_created"), Required]
		public DateTime TimeCreated { get; set; }

		[Column("time_responed")]
		public DateTime? TimeResponded { get; set; }
	}
}

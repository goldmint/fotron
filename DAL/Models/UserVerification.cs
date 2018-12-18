using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fotron.DAL.Models.Base;

namespace Fotron.DAL.Models {

	[Table("ft_user_verification")]
	public class UserVerification : DbBaseUserEntity {

		[Column("first_name"), MaxLength(64)]
		public string FirstName { get; set; }

		[Column("middle_name"), MaxLength(64)]
		public string MiddleName { get; set; }

		[Column("last_name"), MaxLength(64)]
		public string LastName { get; set; }

		[Column("dob")]
		public DateTime? DoB { get; set; }

		[Column("phone_number"), MaxLength(32)]
		public string PhoneNumber { get; set; }

		[Column("country"), MaxLength(64)]
		public string Country { get; set; }

		[Column("country_code"), MaxLength(2)]
		public string CountryCode { get; set; }

		[Column("state"), MaxLength(256)]
		public string State { get; set; }

		[Column("city"), MaxLength(256)]
		public string City { get; set; }

		[Column("postal_code"), MaxLength(16)]
		public string PostalCode { get; set; }

		[Column("street"), MaxLength(256)]
		public string Street { get; set; }

		[Column("apartment"), MaxLength(128)]
		public string Apartment { get; set; }

		[Column("time_user_changed")]
		public DateTime? TimeUserChanged { get; set; }

		[Column("last_kyc_ticket_id")]
		public long? KycLastTicketId { get; set; }

		[ForeignKey(nameof(KycLastTicketId))]
		public virtual KycTicket LastKycTicket { get; set; }

		[Column("tos_agreed")]
		public bool? AgreedWithTos { get; set; }

		[Column("proved_residence")]
		public bool? ProvedResidence { get; set; }

		[Column("proved_residence_comment"), MaxLength(512)]
		public string ProvedResidenceComment { get; set; }
	}
}

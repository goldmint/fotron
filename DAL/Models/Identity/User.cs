using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Fotron.DAL.Models.Identity {

	[Table("ft_user")]
	public class User : IdentityUser<long> {

		public User() : base() { }
		public User(string userName) : base(userName) {}

		// ---

		[Column("id")]
		public override long Id { get; set; }

		[Column("username"), MaxLength(256)]
		public override string UserName { get; set; }

		[Column("normalized_username"), MaxLength(256)]
		public override string NormalizedUserName { get; set; }

		[Column("email"), MaxLength(256)]
		public override string Email { get; set; }

		[Column("normalized_email"), MaxLength(256)]
		public override string NormalizedEmail { get; set; }

		[Column("phone_number"), MaxLength(64)]
		public override string PhoneNumber { get; set; }

		[Column("email_confirmed")]
		public override bool EmailConfirmed { get; set; }

		[Column("phone_number_confirmed")]
		public override bool PhoneNumberConfirmed { get; set; }

		[Column("password_hash"), MaxLength(512)]
		public override string PasswordHash { get; set; }

		[Column("lockout_end")]
		public override DateTimeOffset? LockoutEnd { get; set; }

		[Column("concurrency_stamp"), MaxLength(64)]
		public override string ConcurrencyStamp { get; set; }

		[Column("security_stamp"), MaxLength(64)]
		public override string SecurityStamp { get; set; }

		[Column("lockout_enabled")]
		public override bool LockoutEnabled { get; set; }

		[Column("access_failed_count")]
		public override int AccessFailedCount { get; set; }

		[Column("tfa_enabled")]
		public override bool TwoFactorEnabled { get; set; }

		// ---

		[Column("jwt_salt_cab"), MaxLength(64), Required]
		public string JwtSaltCabinet { get; set; }

		[Column("jwt_salt_dbr"), MaxLength(64), Required]
		public string JwtSaltDashboard { get; set; }

		[Column("access_rights"), Required]
		public long AccessRights { get; set; }

		[Column("tfa_secret"), MaxLength(32), Required]
		public string TfaSecret { get; set; }

		[Column("time_registered"), Required]
		public DateTime TimeRegistered { get; set; }


	    public virtual UserOptions UserOptions { get; set; }
	    public virtual UserVerification UserVerification { get; set; }
    }
}

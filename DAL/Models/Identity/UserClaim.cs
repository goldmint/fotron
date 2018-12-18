using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Fotron.DAL.Models.Identity {

    [Table("ft_user_claim")]
	public class UserClaim : IdentityUserClaim<long> {

		public UserClaim() : base() { }

		// ---

		[Column("id")]
		public override int Id { get; set; }

		[Column("user_id")]
		public override long UserId { get; set; }

		[Column("claim_type"), MaxLength(256)]
		public override string ClaimType { get; set; }

		[Column("claim_value"), MaxLength(256)]
		public override string ClaimValue { get; set; }
	}
}

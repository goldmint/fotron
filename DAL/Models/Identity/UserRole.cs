using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Fotron.DAL.Models.Identity {

    [Table("ft_user_role")]
	public class UserRole : IdentityUserRole<long> {

		public UserRole() : base() { }

		// ---

		[Column("user_id")]
		public override long UserId { get; set; }

		[Column("role_id")]
		public override long RoleId { get; set; }
	}
}

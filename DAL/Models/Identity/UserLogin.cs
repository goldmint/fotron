using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Fotron.DAL.Models.Identity {

    [Table("ft_user_login")]
	public class UserLogin : IdentityUserLogin<long> {

		public UserLogin() : base() {}

		// ---

		[Column("login_provider"), MaxLength(128)]
		public override string LoginProvider { get; set; }

		[Column("provider_key"), MaxLength(128)]
		public override string ProviderKey { get; set; }

		[Column("provider_display_name"), MaxLength(64)]
		public override string ProviderDisplayName { get; set; }

		[Column("user_id")]
		public override long UserId { get; set; }
	}
}

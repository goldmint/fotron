using System.ComponentModel.DataAnnotations.Schema;
using Fotron.DAL.Models.Base;

namespace Fotron.DAL.Models {

	[Table("ft_user_options")]
	public class UserOptions : DbBaseUserEntity {

		[Column("init_tfa_quest")]
		public bool InitialTfaQuest { get; set; }
	}
}

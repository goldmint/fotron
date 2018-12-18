using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Fotron.WebApplication.Models.API.v1.User.UserModels {

	public class AuthenticateModel : BaseValidableModel {

		/// <summary>
		/// Email or username /u[0-9]+/
		/// </summary>
		[Required]
		public string Username { get; set; }

		/// <summary>
		/// Password /.{6,128}/
		/// </summary>
		[Required]
		public string Password { get; set; }

		/// <summary>
		/// Captcha /.{1,1024}/ or null
		/// </summary>
		public string Captcha { get; set; }

		/// <summary>
		/// Valid audience or null
		/// </summary>
		public string Audience { get; set; }

		// ---

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<AuthenticateModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.Username)
				.Must(Common.ValidationRules.BeValidUsername).WithMessage("Invalid format")
				.When(_ => !(_.Username?.Contains("@") ?? false))
			;

			v.RuleFor(_ => _.Username)
				.EmailAddress().WithMessage("Invalid format")
				.Must(Common.ValidationRules.BeValidEmailLength).WithMessage("Invalid length")
				.When(_ => (_.Username?.Contains("@") ?? false))
			;

			v.RuleFor(_ => _.Password)
				.Must(Common.ValidationRules.BeValidPasswordLength).WithMessage("Invalid length")
			;

			v.RuleFor(_ => _.Captcha)
				.Must(Common.ValidationRules.BeValidCaptchaLength).When(_ => !string.IsNullOrWhiteSpace(_.Captcha)).WithMessage("Invalid length")
			;

			return v.Validate(this);
		}
	}

	public class AuthenticateView {

		/// <summary>
		/// Access token
		/// </summary>
		[Required]
		public string Token { get; set; }

		/// <summary>
		/// User have to complete 2 factor auth
		/// </summary>
		[Required]
		public bool TfaRequired { get; set; }
	}

	// ---

	public class RefreshView {

		/// <summary>
		/// Fresh access token
		/// </summary>
		[Required]
		public string Token { get; set; }
	}

	// ---

	public class TfaModel : BaseValidableModel {

		/// <summary>
		/// Two factor auth code /[0-9]{6}/
		/// </summary>
		[Required]
		public string Code { get; set; }

		// ---

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<TfaModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.Code)
				.Must(Common.ValidationRules.BeValidTfaCode).WithMessage("Invalid format")
			;

			return v.Validate(this);
		}
	}

	// ---

	public class DpaCheckModel : BaseValidableModel {

		/// <summary>
		/// Token
		/// </summary>
		[Required]
		public string Token { get; set; }

		// ---

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<DpaCheckModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.Token)
				.Must(Common.ValidationRules.BeValidConfirmationTokenLength).WithMessage("Invalid length")
				; ;

			return v.Validate(this);
		}
	}

	// ---

	public class ProfileView {

		/// <summary>
		/// Id
		/// </summary>
		[Required]
		public string Id { get; set; }

		/// <summary>
		/// Fullname
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// Email
		/// </summary>
		[Required]
		public string Email { get; set; }

		/// <summary>
		/// Has DPA signed
		/// </summary>
		[Required]
		public bool DpaSigned { get; set; }

		/// <summary>
		/// TFA enabled for this user
		/// </summary>
		[Required]
		public bool TfaEnabled { get; set; }
		
		/// <summary>
		/// Has extra-rights
		/// </summary>
		[Required]
		public bool HasExtraRights { get; set; }

		/// <summary>
		/// Level 0 verification is completed
		/// </summary>
		[Required]
		public bool VerifiedL0 { get; set; }

		/// <summary>
		/// Level 1 verification is completed
		/// </summary>
		[Required]
		public bool VerifiedL1 { get; set; }

		/// <summary>
		/// User challenges to pass through
		/// </summary>
		[Required]
		public string[] Challenges { get; set; }
	}

	// ---

	public class ActivityModel : BasePagerModel {

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<ActivityModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			return v.Validate(this);
		}
	}

	public class ActivityView : BasePagerView<ActivityViewItem> {
	}

	public class ActivityViewItem {

		/// <summary>
		/// Type of activity
		/// </summary>
		[Required]
		public string Type { get; set; }

		/// <summary>
		/// Activity comment
		/// </summary>
		[Required]
		public string Comment { get; set; }

		/// <summary>
		/// Client IP
		/// </summary>
		[Required]
		public string Ip { get; set; }

		/// <summary>
		/// Client agent
		/// </summary>
		[Required]
		public string Agent { get; set; }

		/// <summary>
		/// Unixtime
		/// </summary>
		[Required]
		public long Date { get; set; } 
	}

	// ---

	// REMOVE / RENAME
	public class FiatHistoryModel : BasePagerModel {

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<FiatHistoryModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			return v.Validate(this);
		}
	}

	public class FiatHistoryView : BasePagerView<FiatHistoryViewItem> {
	}

	public class FiatHistoryViewItem {

		/// <summary>
		/// Type: deposit, withdraw, goldbuy, goldsell, hwtransfer
		/// </summary>
		[Required]
		public string Type { get; set; }
		
		/// <summary>
		/// Status: 1 - pending, 2 - successful, 3 - cancelled
		/// </summary>
		[Required]
		public int Status { get; set; }

		/// <summary>
		/// Comment
		/// </summary>
		[Required]
		public string Comment { get; set; }

		/// <summary>
		/// Operation source
		/// </summary>
		[Required]
		public string Src { get; set; }
		
		/// <summary>
		/// Operation source amount, optional
		/// </summary>
		public string SrcAmount { get; set; }

		/// <summary>
		/// Unixtime
		/// </summary>
		[Required]
		public long Date { get; set; }

		/// <summary>
		/// Operation destination, optional
		/// </summary>
		public string Dst { get; set; }
		
		/// <summary>
		/// Operation destination, optional
		/// </summary>
		public string DstAmount { get; set; }

		/// <summary>
		/// Tron transaction ID to track, optional
		/// </summary>
		public string EthTxId { get; set; }
	}

	// ---

	public class ZendeskSsoView {

		/// <summary>
		/// JWT to use as Zendesk-SSO payload
		/// </summary>
		[Required]
		public string Jwt { get; set; }
	}

}

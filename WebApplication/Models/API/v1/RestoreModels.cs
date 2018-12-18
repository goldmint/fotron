using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Fotron.WebApplication.Models.API.v1.RestoreModels {

	public class RestoreModel : BaseValidableModel {

		/// <summary>
		/// Valid email /.{,256}/
		/// </summary>
		[Required]
		public string Email { get; set; }

		/// <summary>
		/// Captcha /.{1,1024}/
		/// </summary>
		[Required]
		public string Captcha { get; set; }

		// ---

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<RestoreModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.Email)
				.EmailAddress().WithMessage("Invalid format")
				.Must(Common.ValidationRules.BeValidEmailLength).WithMessage("Invalid length")
			;

			v.RuleFor(_ => _.Captcha)
				.Must(Common.ValidationRules.BeValidCaptchaLength).WithMessage("Invalid length")
			;

			return v.Validate(this);
		}
	}
	
	public class NewPasswordModel : BaseValidableModel {

		/// <summary>
		/// Confirmation token /.{1,512}/
		/// </summary>
		[Required]
		public string Token { get; set; }

		/// <summary>
		/// New password /.{6,128}/
		/// </summary>
		[Required]
		public string Password { get; set; }

		// ---

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<NewPasswordModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.Token)
				.Must(Common.ValidationRules.BeValidConfirmationTokenLength).WithMessage("Invalid length")
			;

			v.RuleFor(_ => _.Password)
				.Must(Common.ValidationRules.BeValidPasswordLength).WithMessage("Invalid length")
			;

			return v.Validate(this);
		}
	}

}

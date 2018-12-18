using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Fotron.WebApplication.Models.API.v1.OAuthModels {

	public class RedirectView {

		/// <summary>
		/// Redirect
		/// </summary>
		public string Redirect { get; set; }
	}

	// ---

	public class ConfirmModel : BaseValidableModel {

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
			var v = new InlineValidator<ConfirmModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.Email)
				.EmailAddress().WithMessage("Invalid format")
				.Must(Common.ValidationRules.BeValidEmailLength)
			;

			v.RuleFor(_ => _.Captcha)
				.Must(Common.ValidationRules.BeValidCaptchaLength).WithMessage("Invalid length")
			;

			return v.Validate(this);
		}
	}

	public class ConfirmView {

		/// <summary>
		/// Access token on success or null in case of email should be confirmed
		/// </summary>
		public string Token { get; set; }

		/// <summary>
		/// Email should be confirmed
		/// </summary>
		public bool ConfirmEmail { get; set; }
	}
}

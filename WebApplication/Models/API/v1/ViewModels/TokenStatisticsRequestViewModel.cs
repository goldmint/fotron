using FluentValidation;
using System.ComponentModel.DataAnnotations;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Fotron.WebApplication.Models.API.v1.ViewModels
{
	public class TokenStatisticsRequestViewModel : RequestByIdViewModel
    {
		[Required]
		public double DateFrom { get; set; }

		public double DateTo { get; set; }

		protected override ValidationResult ValidateFields()
		{
		    base.ValidateFields();

			var v = new InlineValidator<TokenStatisticsRequestViewModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			//v.RuleFor(_ => _.DateFrom).Must(Common.ValidationRules.BeValidDate).WithMessage("Invalid date from");

			//v.RuleFor(_ => _.DateTo).Must(Common.ValidationRules.BeValidDate).WithMessage("Invalid date to");


			return v.Validate(this);
		}
	}
}

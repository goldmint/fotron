using FluentValidation;
using System.ComponentModel.DataAnnotations;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Fotron.WebApplication.Models.API.v1.ViewModels
{
    public class RequestByIdViewModel : BaseValidableModel
    {
        [Required]
        public long Id { get; set; }

        protected override ValidationResult ValidateFields()
        {
            var v = new InlineValidator<RequestByIdViewModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

            v.RuleFor(_ => _.Id).Must(Common.ValidationRules.BeValidId).WithMessage("Invalid token id");

            return v.Validate(this);
        }
    }
}

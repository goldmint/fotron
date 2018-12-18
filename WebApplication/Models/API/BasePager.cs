using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fotron.WebApplication.Models.API {

	public abstract class BasePagerModel {

		/// <summary>
		/// Page offset
		/// </summary>
		[Required]
		public long Offset { get; set; }

		/// <summary>
		/// Page limit (50 max)
		/// </summary>
		[Required]
		public long Limit { get; set; }

		/// <summary>
		/// Sort by known field
		/// </summary>
		[Required]
		public string Sort { get; set; }

		/// <summary>
		/// Ascending sort
		/// </summary>
		public bool Ascending { get; set; }

		// ---

		public static bool IsInvalid<T>(T model, IEnumerable<string> allowedSortValues, out IList<BaseValidableModel.Error> errors) where T : BasePagerModel {
			errors = new List<BaseValidableModel.Error>();

			if (model == null) {
				errors.Add(new BaseValidableModel.Error() {
					Field = "",
					Desc = "Data has invalid format",
				});
				return true;
			}

			if (model.Offset < 0) model.Offset = 0;
			if (model.Limit > 50) model.Limit = 50;
			if (model.Limit < 1) model.Limit = 1;
			model.Sort = model.Sort?.ToLower();

			var v = model.ValidateFields();
			if (v.IsValid) {
				v = model.ValidateSortField(allowedSortValues);
			}

			if ((v.Errors?.Count ?? 0) > 0) {
				foreach (var e in v.Errors) {
					errors.Add(new BaseValidableModel.Error() {
						Field = e.PropertyName,
						Desc = e.ErrorMessage,
					});
				}
			}

			return !v.IsValid;
		}

		public static bool IsInvalid<T>(T model, IEnumerable<string> allowedSortValues) where T : BasePagerModel {
			return IsInvalid(model, allowedSortValues, out var dummy);
		}

		private FluentValidation.Results.ValidationResult ValidateSortField(IEnumerable<string> allowed) {

			var v = new InlineValidator<BasePagerModel>();

			v.RuleFor(_ => _.Sort)
				.Must((x) => x != null && (allowed == null || allowed.Count() == 0 || allowed.Contains(x)))
				.WithMessage("Invalid sort value. Valid are: " + string.Join(',', allowed))
			;

			return v.Validate(this);
		}

		protected abstract FluentValidation.Results.ValidationResult ValidateFields();
	}

	public abstract class BasePagerView<T> {

		/// <summary>
		/// Items
		/// </summary>
		public T[] Items { get; set; }

		/// <summary>
		/// Page offset
		/// </summary>
		public long Offset { get; set; }
		
		/// <summary>
		/// Page limit
		/// </summary>
		public long Limit { get; set; }
		
		/// <summary>
		/// Total items count
		/// </summary>
		public long Total { get; set; }
	}
}

using System.Collections.Generic;

namespace Fotron.WebApplication.Models.API {

	public abstract class BaseValidableModel {

		public static bool IsInvalid<T>(T model, out IList<Error> errors) where T : BaseValidableModel {
			errors = new List<Error>();

			if (model == null) {
				errors.Add(new Error() {
					Field = "",
					Desc = "Data has invalid format",
				});
				return true;
			}

			var v = model.ValidateFields();

			if ((v.Errors?.Count ?? 0) > 0) {
				foreach (var e in v.Errors) {
					errors.Add(new Error() {
						Field = e.PropertyName,
						Desc = e.ErrorMessage,
					});
				}
			}

			return !v.IsValid;
		}

		public static bool IsInvalid<T>(T model) where T : BaseValidableModel {
			return IsInvalid(model, out var dummy);
		}

		protected abstract FluentValidation.Results.ValidationResult ValidateFields();

		// ---

		public sealed class Error {

			public string Field { get; internal set; }
			public string Desc { get; internal set; }
		}
	}
}

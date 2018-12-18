using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Fotron.WebApplication.Core.Response {

	public class APIResponse : Microsoft.AspNetCore.Mvc.ObjectResult {

		public sealed class Response {

			/// <summary>
			/// Service error code or null on success
			/// </summary>
			[Required]
			[Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
			public APIErrorCode? ErrorCode { get; set; }

			/// <summary>
			/// Service error additional description or null on success
			/// </summary>
			[Required]
			[Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
			public string ErrorDesc { get; set; }

			/// <summary>
			/// Result payload or empty object
			/// </summary>
			[Required]
			public object Data { get; set; }
		}

		private readonly HttpStatusCode _httpStatusCode;
		private readonly Response _response;

		// ---

		private APIResponse(HttpStatusCode statusCode, Response response) : base(response) {
			_httpStatusCode = statusCode;
			_response = response;

			StatusCode = (int)statusCode;
			DeclaredType = typeof(Response);
			ContentTypes.Clear();
			ContentTypes.Add("application/json");
		}

		public async Task WriteResponse(HttpContext context) {
			context.Response.StatusCode = StatusCode.Value;
			context.Response.ContentType = ContentTypes[0];

			await context.Response.WriteAsync(
				Newtonsoft.Json.JsonConvert.SerializeObject(
					Value,
					Common.Json.CamelCaseSettings
				)
			);
		}

		// ---

		public HttpStatusCode GetHttpStatusCode() {
			return _httpStatusCode;
		}

		public APIErrorCode? GetErrorCode() {
			return _response.ErrorCode;
		}

		// ---

		/// <summary>
		/// 200: Everyting worked as expected
		/// </summary>
		public static APIResponse Success(object result = null) {
			return new APIResponse(HttpStatusCode.OK, new Response() {
				ErrorCode = null,
				ErrorDesc = null,
				Data = result ?? new object(),
			});
		}

		/// <summary>
		/// 400: Failed request result or invalid data passed
		/// </summary>
		public static APIResponse BadRequest(IEnumerable<Fotron.WebApplication.Models.API.BaseValidableModel.Error> errorFields, string format = null, params object[] args) {
			return new APIResponse(HttpStatusCode.BadRequest, new Response() {
				ErrorCode = APIErrorCode.InvalidParameter,
				ErrorDesc = APIErrorCode.InvalidParameter.ToDescription(format),
				Data = errorFields ?? new object(),
			});
		}

		/// <summary>
		/// 400: Failed request result or invalid data passed
		/// </summary>
		public static APIResponse BadRequest(string errorField, string errorDescription, string format = null, params object[] args) {
			return BadRequest(
				new[] { new Fotron.WebApplication.Models.API.BaseValidableModel.Error() {
					Field = errorField,
					Desc = errorDescription,
				}},
				format, args
			);
		}

		/// <summary>
		/// 400: Failed request result or invalid data passed
		/// </summary>
		public static APIResponse BadRequest(APIErrorCode code, object result, string format = null, params object[] args) {
			return new APIResponse(HttpStatusCode.BadRequest, new Response() {
				ErrorCode = code,
				ErrorDesc = code.ToDescription(format),
				Data = result ?? new object(),
			});
		}

		/// <summary>
		/// 400: Failed request result or invalid data passed
		/// </summary>
		public static APIResponse BadRequest(APIErrorCode code, string format = null, params object[] args) {
			return new APIResponse(HttpStatusCode.BadRequest, new Response() {
				ErrorCode = code,
				ErrorDesc = code.ToDescription(format),
				Data = new object(),
			});
		}

		/// <summary>
		/// 500: Server failure
		/// </summary>
		public static APIResponse Failure(APIErrorCode code, object result = null, string format = null, params object[] args) {
			return new APIResponse(HttpStatusCode.InternalServerError, new Response() {
				ErrorCode = code,
				ErrorDesc = code.ToDescription(format),
				Data = result ?? new object(),
			});
		}

		/// <summary>
		/// 500: General server failure without any payload
		/// </summary>
		public static APIResponse GeneralInternalFailure(Exception e = null, bool showError = false) {
			return new APIResponse(HttpStatusCode.InternalServerError, new Response() {
				ErrorCode = APIErrorCode.InternalServerError,
				ErrorDesc = APIErrorCode.InternalServerError.ToDescription("Unexpected service failure. Please retry later"),
				Data = showError && e != null ? e : new object(),
			});
		}
	}
}

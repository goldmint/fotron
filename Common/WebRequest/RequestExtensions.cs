using System;
using System.Net.Http;
using System.Text;

namespace Fotron.Common.WebRequest {

	public static class RequestExtensions {

		public static Request AcceptJson(this Request req) {
			req.Accept(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			return req;
		}

		// ---

		public static Request BodyForm(this Request req, string body) {
			req.Body(new StringContent(body ?? "", Encoding.UTF8, "application/x-www-form-urlencoded"));
			return req;
		}

		public static Request BodyForm(this Request req, Parameters body) {
			req.Body(new StringContent(body?.ToUrlEncoded() ?? "", Encoding.UTF8, "application/x-www-form-urlencoded"));
			return req;
		}

		public static Request BodyJson(this Request req, string json) {
			req.Body(new StringContent(json ?? "{}", Encoding.UTF8, "application/json"));
			return req;
		}

		public static Request BodyJson(this Request req, object json) {
			req.Body(new StringContent(Json.Stringify(json), Encoding.UTF8, "application/json"));
			return req;
		}

		public static Request BodyJsonRpc(this Request req, string method, object paramz, object id = null) {

			var json = new {
				jsonrpc = "2.0",
				method = method,
				@params = paramz,
				id = id ?? Guid.NewGuid().ToString("N"),
			};

			var jsonStr = Json.Stringify(json);

			req.Body(new StringContent(jsonStr, Encoding.UTF8, "application/json"));
			return req;
		}

	}
}

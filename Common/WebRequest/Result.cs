using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fotron.Common.WebRequest {

	public sealed class Result : IDisposable {

		private HttpResponseMessage _responseRef;
		private HttpContent _contentRef;
		private HttpContent _content;

		public Result(HttpResponseMessage response) {
			_responseRef = response;
			_content = response.Content;
		}

		public Result(HttpContent content) {
			_contentRef = content;
			_content = content;
		}

		public void Dispose() {
			DisposeManaged();
		}

		private void DisposeManaged() {
			_responseRef?.Dispose();
			_contentRef?.Dispose();
		}

		// ---

		public HttpStatusCode? GetHttpStatus() {
			return _responseRef?.StatusCode;
		}

		public async Task<string> ToRawString() {
			return await _content.ReadAsStringAsync();
		}

	}
}

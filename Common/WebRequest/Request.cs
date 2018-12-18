using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Core;
using NLog;

namespace Fotron.Common.WebRequest {

	public sealed partial class Request : IDisposable {

		private static readonly TimeSpan Timeout10 = TimeSpan.FromSeconds(10);
		private static readonly TimeSpan Timeout30 = TimeSpan.FromSeconds(30);

		private HttpContent _body;
		private string _query;
		private AuthenticationHeaderValue _auth;
		private List<MediaTypeWithQualityHeaderValue> _hdrAccept;
		private Func<Result, Task> _callbackFnc;
		private Action<Result> _callbackAct;
		private ILogger _logger;
		private CancellationTokenSource _cancellationTokenSource;

		public Request(ILogger logger = null) {
			_logger = logger;
			_hdrAccept = new List<MediaTypeWithQualityHeaderValue>();
			_cancellationTokenSource = new CancellationTokenSource();
		}

		public void Dispose() {
			DisposeManaged();
		}

		private void DisposeManaged() {
			_cancellationTokenSource?.Dispose();
			_body?.Dispose();
		}

		// ---

		public Request Query(Parameters query) {
			_query = query?.ToUrlEncoded();
			return this;
		}

		public Request Query(string query) {
			_query = query;
			return this;
		}

		public Request AuthBasic(string auth, bool convertToBase64) {
			if (convertToBase64) {
				auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
			}
			_auth = new AuthenticationHeaderValue("Basic", auth);
			return this;
		}

		public Request AuthToken(string token) {
			_auth = new AuthenticationHeaderValue("Token", token);
			return this;
		}

		public Request Accept(MediaTypeWithQualityHeaderValue accept) {
			_hdrAccept.Add(accept);
			return this;
		}

		public Request Body(HttpContent body) {
			_body = body;
			return this;
		}

		public Request OnResult(Func<Result, Task> cbk) {
			_callbackFnc = cbk;
			_callbackAct = null;
			return this;
		}

		public Request OnResult(Action<Result> cbk) {
			_callbackAct = cbk;
			_callbackFnc = null;
			return this;
		}

		// ---

		public async Task<bool> SendGet(string url, TimeSpan? timeout = null, CancellationToken? ct = null) {
			return await Send(false, url, timeout ?? Timeout10, ct ?? _cancellationTokenSource.Token);
		}

		public async Task<bool> SendPost(string url, TimeSpan? timeout = null, CancellationToken? ct = null) {
			return await Send(true, url, timeout ?? Timeout30, ct ?? _cancellationTokenSource.Token);
		}

		private async Task<bool> Send(bool post, string url, TimeSpan timeout, CancellationToken ct) {

			var urlBuilder = new UriBuilder(url);
			var urlQueryBuilder = new UriQueryBuilder();

			var queryPart1 = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(urlBuilder.Query);
			foreach (var pair in queryPart1) {
				urlQueryBuilder.Add(pair.Key, pair.Value.FirstOrDefault());
			}

			var queryPart2 = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(_query);
			foreach (var pair in queryPart2) {
				urlQueryBuilder.Add(pair.Key, pair.Value.FirstOrDefault());
			}

			urlBuilder.Query = urlQueryBuilder.ToString();
			url = urlBuilder.ToString();

			using (var client = new HttpClient()) {

				client.Timeout = timeout;
				if (_auth != null) {
					client.DefaultRequestHeaders.Authorization = _auth;
				}

				foreach (var ah in _hdrAccept) {
					client.DefaultRequestHeaders.Accept.Add(ah);
				}

				try {
					_logger?.Trace($"Sending request to `{url}`");

					if (post) {
						using (var res = new Result(await client.PostAsync(url, _body, ct))) {
							if (_callbackFnc != null) await _callbackFnc(res);
							_callbackAct?.Invoke(res);
						}
					}
					else {
						using (var res = new Result(await client.GetAsync(url, ct))) {
							if (_callbackFnc != null) await _callbackFnc(res);
							_callbackAct?.Invoke(res);
						}
					}
				} catch (Exception e) {
					_logger?.Error(e, "Failed to send request to " + url);
				}
			}

			return false;
		}
	}
}

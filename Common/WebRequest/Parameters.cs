using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Fotron.Common.WebRequest {

	public class Parameters {

		private readonly Dictionary<string, string> _dict;

		public Parameters() {
			_dict = new Dictionary<string, string>();
		}

		public Dictionary<string, string> GetDictionary() {
			var ret = new Dictionary<string, string>();
			return ((new Parameters()) + this)._dict; // copy
		}

		public Parameters Set(string key, string value) {
			if (key != null && value != null) {
				if (_dict.ContainsKey(key)) {
					_dict.Remove(key);
				}
				_dict.Add(key, value);
			}

			return this;
		}

		public Parameters SetIfEmpty(string key, string value) {
			if (!_dict.ContainsKey(key)) {
				Set(key, value);
			}
			return this;
		}

		public string ToUrlEncoded() {
			var sb = new StringBuilder();
			foreach (var pair in _dict) {
				sb.Append("&");
				sb.Append(WebUtility.UrlEncode(pair.Key));
				sb.Append("=");
				sb.Append(WebUtility.UrlEncode(pair.Value));
			}
			return sb.ToString().TrimStart('&');
		}

		public string ToJson() {
			return Json.Stringify(_dict);
		}

		public static Parameters operator +(Parameters a, Parameters b) {
			var ret = new Parameters();

			foreach (var pair in a._dict) {
				if (pair.Key != null && pair.Value != null) {
					if (ret._dict.ContainsKey(pair.Key)) ret._dict.Remove(pair.Key);
					ret._dict.Add(pair.Key, pair.Value);
				}
			}

			foreach (var pair in b._dict) {
				if (pair.Key != null && pair.Value != null) {
					if (ret._dict.ContainsKey(pair.Key)) ret._dict.Remove(pair.Key);
					ret._dict.Add(pair.Key, pair.Value);
				}
			}

			return ret;
		}
	}

}

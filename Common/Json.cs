using System;
using System.IO;

namespace Fotron.Common {

	public static class Json {

		public static readonly Newtonsoft.Json.JsonSerializerSettings CamelCaseSettings = new Newtonsoft.Json.JsonSerializerSettings() {
			ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver() {
				NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy(),
			},
		};

		public static readonly Newtonsoft.Json.JsonSerializerSettings SnakeCaseSettings = new Newtonsoft.Json.JsonSerializerSettings() {
			ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver() {
				NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy(),
			},
		};

		// ---

		public static object Parse(string str, Newtonsoft.Json.JsonSerializerSettings settings = null) {
			try {
				if (settings != null) {
					return Newtonsoft.Json.JsonConvert.DeserializeObject(str, settings);
				}
				else {
					return Newtonsoft.Json.JsonConvert.DeserializeObject(str);
				}
			}
			catch (Exception) { }
			return null;
		}

		public static T Parse<T>(string str, Newtonsoft.Json.JsonSerializerSettings settings = null) where T : class {
			try {
				if (settings != null) {
					return Newtonsoft.Json.JsonConvert.DeserializeObject(str, typeof(T), settings) as T;
				}
				else {
					return Newtonsoft.Json.JsonConvert.DeserializeObject(str, typeof(T)) as T;
				}
			}
			catch (Exception) { }
			return default(T);
		}

		public static T Parse<T>(Stream stream, Newtonsoft.Json.JsonSerializerSettings settings = null) where T : class {
			try {

				var serializer = new Newtonsoft.Json.JsonSerializer();
				if (settings != null) {
					serializer.ContractResolver = settings.ContractResolver;
				}

				using (var reader = new StreamReader(stream)) {
					return serializer.Deserialize(reader, typeof(T)) as T;
				}
			}
			catch (Exception) { }
			return default(T);
		}

		public static bool ParseAnonymous<T>(string str, T obj, Newtonsoft.Json.JsonSerializerSettings settings = null) where T : class {
			try {
				if (settings != null) {
					Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(str, obj, settings);
				}
				else {
					Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(str, obj);
				}
				return true;
			}
			catch (Exception) { }
			return false;
		}

		public static bool ParseInto(Stream stream, object obj, Newtonsoft.Json.JsonSerializerSettings settings = null) {
			try {

				var serializer = new Newtonsoft.Json.JsonSerializer();
				if (settings != null) {
					serializer.ContractResolver = settings.ContractResolver;
				}

				using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8)) {
					serializer.Populate(reader, obj);
				}
				return true;
			}
			catch (Exception) { }
			return false;
		}

		public static bool ParseInto(string str, object obj, Newtonsoft.Json.JsonSerializerSettings settings = null) {
			try {

				if (settings != null) {
					Newtonsoft.Json.JsonConvert.PopulateObject(str, obj, settings);
				}
				else {
					Newtonsoft.Json.JsonConvert.PopulateObject(str, obj);
				}
				
				return true;
			}
			catch (Exception) { }
			return false;
		}

		public static string Stringify(object obj, Newtonsoft.Json.JsonSerializerSettings settings = null) {
			try {

				if (settings != null) {
					return Newtonsoft.Json.JsonConvert.SerializeObject(obj, settings);
				}
				else {
					return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
				}
			}
			catch (Exception) { }
			return "{}";
		}
	}
}

using System;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace Fotron.Common.Extensions {

	public static class LoggerExtensions {

		public static ILogger GetLoggerFor(this LogFactory factory, object instance) {
			return factory.GetLogger(instance.GetType().Name);
		}

		public static ILogger GetLoggerFor(this IServiceProvider services, Type type) {
			return services.GetRequiredService<LogFactory>().GetLogger(type.Name);
		}

		public static ILogger GetLoggerFor<T>(this IServiceProvider services) {
			return services.GetRequiredService<LogFactory>().GetLogger(typeof(T).Name);
		}
	}
}

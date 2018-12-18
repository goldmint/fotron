using Fotron.Common;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fotron.Common.Extensions;

namespace Fotron.CoreLogic.Services.Mutex.Impl {
#if DEBUG

	public sealed class DebugMutexHolder : IMutexHolder {

		private static ConcurrentDictionary<string, string> _dict = new ConcurrentDictionary<string, string>();
		private readonly ILogger _logger;

		public DebugMutexHolder(LogFactory logFactory) {
			_logger = logFactory.GetLoggerFor(this);
		}

		// ---

		public bool SetMutex(string mutex, string locker, TimeSpan timeout) {
			return setMutex(mutex, locker, timeout);
		}

		public bool UnsetMutex(string mutex, string locker) {
			return unsetMutex(mutex, locker);
		}

		public Task<bool> SetMutexAsync(string mutex, string locker, TimeSpan timeout) {
			return Task.FromResult(setMutex(mutex, locker, timeout));
		}

		public Task<bool> UnsetMutexAsync(string mutex, string locker) {
			return Task.FromResult(unsetMutex(mutex, locker));
		}

		// ---

		private bool setMutex(string mutex, string locker, TimeSpan lockTimeout) {

			if (string.IsNullOrWhiteSpace(mutex)) throw new ArgumentException("Mutex is empty");
			if (string.IsNullOrWhiteSpace(locker)) throw new ArgumentException("Locker is empty");
			if (lockTimeout == null || lockTimeout.TotalSeconds < 1) throw new ArgumentException("Timeout is invalid");

			if (_dict.TryAdd(mutex, locker)) {
				return true;
			}
			else {
				_logger?.Trace("Failed to acquire mutex `{0}` for locker `{1}`", mutex, locker);
				return false;
			}

		}

		private bool unsetMutex(string mutex, string locker) {
			if (string.IsNullOrWhiteSpace(mutex)) throw new ArgumentException("Mutex is empty");
			if (string.IsNullOrWhiteSpace(locker)) throw new ArgumentException("Locker is empty");

			string outlocker;
			return _dict.TryRemove(mutex, out outlocker);
		}
	}

#endif
}

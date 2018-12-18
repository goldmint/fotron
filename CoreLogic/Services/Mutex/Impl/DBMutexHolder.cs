using Fotron.Common;
using Fotron.DAL;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Threading.Tasks;
using Fotron.Common.Extensions;

namespace Fotron.CoreLogic.Services.Mutex.Impl {

	public sealed class DBMutexHolder : IMutexHolder {

		private readonly ILogger _logger;
		private readonly ApplicationDbContext _dbContext;

		public DBMutexHolder(ApplicationDbContext dbContext, LogFactory logFactory) {
			_logger = logFactory.GetLoggerFor(this);
			_dbContext = dbContext;
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
			bool success = false;

			if (string.IsNullOrWhiteSpace(mutex)) throw new ArgumentException("Mutex is empty");
			if (string.IsNullOrWhiteSpace(locker)) throw new ArgumentException("Locker is empty");
			if (lockTimeout == null || lockTimeout.TotalSeconds < 1) throw new ArgumentException("Timeout is invalid");

			try {
				// remove expired first
				_dbContext.Database.ExecuteSqlCommand(
					"DELETE FROM gm_mutex WHERE id=@p0 AND expires<@p1",
					mutex, DateTime.UtcNow
				);

				// try insert new
				_dbContext.Database.ExecuteSqlCommand(
					"INSERT INTO gm_mutex (id, locker, expires) VALUES (@p0, @p1, @p2)",
					mutex, locker, DateTime.UtcNow.Add(lockTimeout)
				);
				success = true;
			} catch (Exception e) {
				_logger?.Trace(e, "Failed to acquire mutex `{0}` for locker `{1}`", mutex, locker);
			}

			if (success) {
				_logger?.Debug("Mutexxx `{0}` acquired by `{1}`", mutex, locker);
			}

			return success;
		}

		private bool unsetMutex(string mutex, string locker) {
			bool success = false;

			if (string.IsNullOrWhiteSpace(mutex)) throw new ArgumentException("Mutex is empty");
			if (string.IsNullOrWhiteSpace(locker)) throw new ArgumentException("Locker is empty");

			try {
				var count = _dbContext.Database.ExecuteSqlCommand(
					"DELETE FROM gm_mutex WHERE id=@p0 AND locker=@p1 AND expires>@p2",
					mutex, locker, DateTime.UtcNow
				);
				success = count == 1;
			}
			catch (Exception e) {
				success = false;
				_logger?.Trace(e, "Failed to release mutex `{0}` for locker `{1}`", mutex, locker);
			}

			if (success) {
				_logger?.Debug("Mutexxx `{0}` released by `{1}`", mutex, locker);
			}

			return success;
		}
	}
}

using FluentValidation;
using Fotron.Common;
using System;
using System.Threading.Tasks;

namespace Fotron.CoreLogic.Services.Mutex.Impl {

	public sealed class MutexBuilder {

		private readonly IMutexHolder _holder;
		private string _mutex;
		private string _locker;
		private TimeSpan _timeout;

		public MutexBuilder(IMutexHolder holder) {
			_holder = holder;
			_timeout = TimeSpan.FromMinutes(10);
			LockerGuid();
		}

		/// <summary>
		/// Set name: somemutex_f00ba0
		/// </summary>
		public MutexBuilder Mutex(MutexEntity entity, long id) {
			_mutex = string.Format("{0}_{1}", entity.ToString(), id.ToString("X"));
			return this;
		}

		/// <summary>
		/// Set custom timeout
		/// </summary>
		public MutexBuilder Timeout(TimeSpan timeout) {
			if (timeout.TotalSeconds < 5) throw new ArgumentException("Timeout must be at least 5 second");
			_timeout = timeout;
			return this;
		}

		/// <summary>
		/// Set user locker
		/// </summary>
		public MutexBuilder LockerUser(long userId) {
			if (userId < 1) throw new ArgumentException("User ID must be valid user ID");
			_locker = "user_" + userId.ToString("X");
			return this;
		}

		/// <summary>
		/// Set random unique locker (default locker)
		/// </summary>
		public MutexBuilder LockerGuid() {
			_locker = Guid.NewGuid().ToString("N");
			return this;
		}

		/// <summary>
		/// Lock, execute callback, unlock
		/// </summary>
		public async Task CriticalSection(Action<bool> callback) {
			Validate();

			var result = await _holder.SetMutexAsync(_mutex, _locker, _timeout);

			if (result) {
				try {
					callback(true);
					return;
				}
				finally {
					await _holder.UnsetMutexAsync(_mutex, _locker);
				}
			}

			callback(false);
		}

		/// <summary>
		/// Lock, execute callback, unlock
		/// </summary>
		public async Task CriticalSection(Func<bool, Task> callback) {
			Validate();

			var result = await _holder.SetMutexAsync(_mutex, _locker, _timeout);

			if (result) {
				try {
					await callback(true);
					return;
				}
				finally {
					await _holder.UnsetMutexAsync(_mutex, _locker);
				}
			}

			await callback(false);
		}

		/// <summary>
		/// Lock, execute callback, unlock
		/// </summary>
		public async Task<T> CriticalSection<T>(Func<bool, Task<T>> callback) {
			Validate();

			var result = await _holder.SetMutexAsync(_mutex, _locker, _timeout);

			if (result) {
				try {
					return await callback(true);
				}
				finally {
					await _holder.UnsetMutexAsync(_mutex, _locker);
				}
			}

			return await callback(false);
		}

		/// <summary>
		/// Try enter lock
		/// </summary>
		public async Task<bool> TryEnter() {
			Validate();
			return await _holder.SetMutexAsync(_mutex, _locker, _timeout);
		}

		/// <summary>
		/// Try leave lock
		/// </summary>
		public async Task<bool> TryLeave() {
			Validate();
			return await _holder.UnsetMutexAsync(_mutex, _locker);
		}

		/// <summary>
		/// Try re-enter lock
		/// </summary>
		public async Task<bool> TryReenter() {
			Validate();
			return await TryLeave() && await TryEnter(); // must be atomic actually
		}

		// ---

		private void Validate() {
			_mutex = _mutex?.ToLower().Trim('_', ' ');
			_locker = _locker?.ToLower().Trim('_', ' ');

			new Validator().ValidateAndThrow(this);
		}

		internal class Validator : AbstractValidator<MutexBuilder> {

			public Validator() {
				RuleFor(x => x._mutex).Length(1, 64);
				RuleFor(x => x._locker).Length(1, 32);
				RuleFor(x => x._timeout).NotNull().Must(x => x.TotalSeconds > 0d);
			}
		}
	}
}

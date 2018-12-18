using System;
using System.Threading.Tasks;

namespace Fotron.CoreLogic.Services.Mutex {

	public interface IMutexHolder {

		bool SetMutex(string mutex, string locker, TimeSpan timeout);
		Task<bool> SetMutexAsync(string mutex, string locker, TimeSpan timeout);

		bool UnsetMutex(string mutex, string locker);
		Task<bool> UnsetMutexAsync(string mutex, string locker);
	}
}

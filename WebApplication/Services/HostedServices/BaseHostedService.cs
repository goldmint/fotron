using System;
using System.Threading;
using System.Threading.Tasks;
using Fotron.Common;
using Fotron.Common.Extensions;
using Fotron.CoreLogic.Services.Blockchain.Tron;
using Fotron.DAL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Fotron.WebApplication.Services.HostedServices {

	public abstract class BaseHostedService : IHostedService, IDisposable {

		protected AppConfig AppConfig { get; }
		protected ILogger Logger { get; }
		protected ApplicationDbContext DbContext { get; }
		protected ITronReader TronObserver { get; }
		protected ITronWriter TronWriter { get; }

		protected abstract TimeSpan Period { get; }
		private Task _task;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();

		protected BaseHostedService(IServiceProvider services) {
			Logger = services.GetLoggerFor(this.GetType());
			AppConfig = services.GetRequiredService<AppConfig>();
			DbContext = services.GetRequiredService<ApplicationDbContext>();
			TronObserver = services.GetRequiredService<ITronReader>();
			TronWriter = services.GetRequiredService<ITronWriter>();
		}

		public void Dispose() {
		}

		public Task StartAsync(CancellationToken cancellationToken) {
			_task = ExecuteAsync(_cts.Token);
			return _task.IsCompleted ? _task : Task.CompletedTask;
		}

		public async Task StopAsync(CancellationToken cancellationToken) {
			if (_task == null) {
				return;
			}
			try {
				_cts.Cancel();
			}
			finally {
				await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, cancellationToken));
			}
		}

		private async Task ExecuteAsync(CancellationToken cancellationToken) {
			await OnInit();

			while (!cancellationToken.IsCancellationRequested) {
				await DoWork();
				await Task.Delay(Period, cancellationToken);
			}
		}

		protected virtual Task OnInit() => Task.CompletedTask;
		protected abstract Task DoWork();

	}
}

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

namespace Fotron.WebApplication.Services.HostedServices
{
	public abstract class BaseHostedService : IHostedService, IDisposable
	{
		protected AppConfig AppConfig { get; }
		protected ILogger Logger { get; }
		protected ApplicationDbContext DbContext { get; }
		protected ITronReader TronObserver { get; }
	    protected ITronWriter TronWriter { get; }

        protected abstract TimeSpan Period { get; }

		private Timer _timer;

		protected BaseHostedService(IServiceProvider services) {
			Logger = services.GetLoggerFor(this.GetType());
			AppConfig = services.GetRequiredService<AppConfig>();
			DbContext = services.GetRequiredService<ApplicationDbContext>();
			TronObserver = services.GetRequiredService<ITronReader>();
		    TronWriter = services.GetRequiredService<ITronWriter>();
		}

		public async Task StartAsync(CancellationToken cancellationToken) {
			await OnInit();

			_timer = new Timer(DoWork, null, TimeSpan.Zero, Period);
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		protected abstract void DoWork(object state);

		protected virtual Task OnInit() {
			return Task.CompletedTask;
		}

		public void Dispose() {
			_timer?.Dispose();
		}

	}
}

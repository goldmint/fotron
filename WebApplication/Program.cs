using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace Fotron.WebApplication {

	public class Program {

		private static IWebHost _webHost;

		public static void Main(string[] args) {

			AppDomain.CurrentDomain.ProcessExit += OnTermination;
			_webHost = BuildWebHost(args);
			_webHost.Run();
		}

		public static IWebHost BuildWebHost(string[] args) {
			return WebHost.CreateDefaultBuilder(args)
				.UseKestrel(opts => {
					opts.AddServerHeader = false;
					opts.UseSystemd();
					opts.AllowSynchronousIO = true;
					opts.ApplicationSchedulingMode = Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal.SchedulingMode.ThreadPool;
				})
				.UseLibuv()
				.UseStartup<Startup>()
				.Build()
			;
		}

		public static void OnTermination(object sender, EventArgs e) {
			_webHost.StopAsync();
			_webHost.WaitForShutdown();
		}
	}
}

using System;
using System.Threading.Tasks;

namespace Fotron.WebApplication.Services.HostedServices
{
    public class MaxGasPriceUpdater : BaseHostedService
    {
        protected override TimeSpan Period => TimeSpan.FromMinutes(20);
        
        public MaxGasPriceUpdater(IServiceProvider services) : base(services) { }


        protected override async void DoWork(object state)
        {
            //var gasPrice = await TronObserver.GetCurrentGasPrice();

            //await TronWriter.UpdateMaxGaxPrice(gasPrice);
        }
    }
}

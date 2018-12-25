using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fotron.Common;
using Fotron.DAL.Models;
using Fotron.WebApplication.Core.Policies;
using Fotron.WebApplication.Core.Response;
using Fotron.WebApplication.Models.API.v1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fotron.WebApplication.Controllers.v1
{
    [Route("api/v1/token")]
    public class TokenController : BaseController
    {
        [AnonymousAccess]
        [HttpGet, Route("list")]
        [ProducesResponseType(typeof(TokenBaseInfoResponseViewModel[]), 200)]
        public async Task<APIResponse> GetTokenList()
        {

	        var query = DbContext.Tokens.Where(x => x.IsEnabled && !x.IsDeleted);

	        var list = new List<TokenBaseInfoResponseViewModel>();

	        (await query.AsNoTracking().ToListAsync()).ForEach(x => list.Add(Mapper.Map<TokenBaseInfoResponseViewModel>(x)));

	        foreach (var token in list)
	        {
		        var last7DStatList = await DbContext.TokenStatistics.Where(x => x.TokenId == token.Id && x.Date >= DateTime.Now.AddDays(-7)).ToListAsync();
		        token.PriceStatistics7D = last7DStatList.Select(x => x.PriceEth).ToList();
                
		        if (last7DStatList.Count > 1) {
			        var prevDayPrice = last7DStatList[last7DStatList.Count - 2].PriceEth;
			        if (prevDayPrice > 0) {
				        token.PriceChangeLastDayPercent = (((token.CurrentPriceEth - prevDayPrice) / prevDayPrice) * 100).RoundUp(2);
			        }
			        token.TradingVolume24HEth = (last7DStatList[last7DStatList.Count - 1].VolumeEth - last7DStatList[last7DStatList.Count - 2].VolumeEth).RoundUp(2);
		        }
	        }

	        return APIResponse.Success(list);

        }

        [AnonymousAccess]
        [HttpGet, Route("full-info")]
        [ProducesResponseType(typeof(TokenFullInfoResponseViewModel[]), 200)]
        public async Task<APIResponse> GetTokenFullInfo(RequestByIdViewModel viewModel)
        {
            var item = await DbContext.Tokens.FirstOrDefaultAsync(x => x.Id == viewModel.Id && x.IsEnabled && !x.IsDeleted);

            var res = Mapper.Map<TokenFullInfoResponseViewModel>(item);

            return APIResponse.Success(res);
        }

        [AnonymousAccess]
        [HttpGet, Route("stat")]
        [ProducesResponseType(typeof(TokenStatisticsResponseViewModel[]), 200)]
        public async Task<APIResponse> GetTokenStatistics(TokenStatisticsRequestViewModel viewModel)
        {
            var query = DbContext.TokenStatistics.Where(x => x.TokenId == viewModel.Id && x.Date >= Utils.UnixTimestampToDateTime(viewModel.DateFrom));

            if (viewModel.DateTo > 0) query = query.Where(x => x.Date <= Utils.UnixTimestampToDateTime(viewModel.DateTo));
            
            var list = new List<TokenStatisticsResponseViewModel>();

            (await query.AsNoTracking().ToListAsync()).ForEach(x => list.Add(Mapper.Map<TokenStatisticsResponseViewModel>(x)));

            return APIResponse.Success(list);
        }

        [AnonymousAccess]
        [HttpPost, Route("add-request")]
        public async Task<APIResponse> AddRequest([FromBody] AddTokenRequestViewModel viewModel)
        {
            var model = Mapper.Map<AddTokenRequest>(viewModel);

            model.IsEnabled = true;
            model.TimeCreated = DateTime.Now;

            DbContext.AddTokenRequests.Add(model);

            await DbContext.SaveChangesAsync();

	        try {
		        var body = $@"
					Hello from fotron.io
					There is a new request:

					Company: {model.CompanyName}
					Email: {model.ContactEmail}
					Website: {model.WebsiteUrl}
					
					Token ticker: {model.TokenTicker}
					Token contract address: {model.TokenContractAddress}
					Start price: {model.StartPriceEth} ETH
					Total token supply: {model.TotalSupply}
				";
		        await EmailSender.Send(AppConfig.Apps.AdminEmails, "New request", body.Replace("\t", ""));
	        }
	        catch {
	        }

	        return APIResponse.Success();
        }
    }
}

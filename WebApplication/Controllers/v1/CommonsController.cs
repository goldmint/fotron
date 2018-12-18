using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Fotron.WebApplication.Core.Policies;
using Fotron.WebApplication.Core.Response;

namespace Fotron.WebApplication.Controllers.v1 {

	[Route("api/v1/commons")]
	public class CommonsController : BaseController {

		/// <summary>
		/// Countries blacklist
		/// </summary>
		[AnonymousAccess]
		[HttpGet, Route("bannedCountries")]
		[ProducesResponseType(typeof(string[]), 200)]
		public async Task<APIResponse> BannedCountries() {
			var list = await (
				from a in DbContext.BannedCountry
				select a.Code.ToUpper()
			).AsNoTracking().ToArrayAsync();
			return APIResponse.Success(list);
		}
	}
}
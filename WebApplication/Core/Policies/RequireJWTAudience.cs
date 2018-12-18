using Fotron.Common;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Fotron.WebApplication.Core.Policies {

	public class RequireJWTAudience : IAuthorizationRequirement {

		private JwtAudience _audience;

		public RequireJWTAudience(JwtAudience audience) {
			_audience = audience;
		}

		public class Handler : AuthorizationHandler<RequireJWTAudience> {

			protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireJWTAudience requirement) {
				if (context.User.HasClaim("aud", requirement._audience.ToString().ToLower())) {
					context.Succeed(requirement);
					return Task.CompletedTask;
				}

				context.Fail();
				return Task.CompletedTask;
			}
		}
	}
}

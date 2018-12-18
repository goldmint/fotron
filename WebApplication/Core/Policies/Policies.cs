using System;
using Microsoft.AspNetCore.Authorization;

namespace Fotron.WebApplication.Core.Policies {

	public static class Policy {

		/// <summary>
		/// Audience constraint
		/// </summary>
		public const string JWTAudienceTemplate = "PolicyJWTAudience_";

		/// <summary>
		/// Area constraint
		/// </summary>
		public const string JWTAreaTemplate = "PolicyJWTArea_";

		/// <summary>
		/// Access rights constraint
		/// </summary>
		public const string AccessRightsTemplate = "PolicyAccessRights_";
	}

	// ---

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class AnonymousAccessAttribute : Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute {
		public AnonymousAccessAttribute() : base() { }
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class RequireJWTAudienceAttribute : AuthorizeAttribute {
		public RequireJWTAudienceAttribute(Common.JwtAudience aud) : base(Policies.Policy.JWTAudienceTemplate + aud.ToString()) { }
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RequireJWTAreaAttribute : AuthorizeAttribute {
		public RequireJWTAreaAttribute(Common.JwtArea area) : base(Policies.Policy.JWTAreaTemplate + area.ToString()) { }
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RequireAccessRightsAttribute : AuthorizeAttribute {
		public RequireAccessRightsAttribute(Common.AccessRights rights) : base(Policies.Policy.AccessRightsTemplate + rights.ToString()) { }
	}
}

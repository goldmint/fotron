using AutoMapper;
using Fotron.Common;
using Fotron.CoreLogic.Services.Blockchain.Tron;
using Fotron.CoreLogic.Services.Blockchain.Tron.Impl;
using Fotron.DAL;
using Fotron.DAL.Models.Identity;
using Fotron.WebApplication.Core;
using Fotron.WebApplication.Core.Policies;
using Fotron.WebApplication.Core.Tokens;
using Fotron.WebApplication.Services.Email;
using Fotron.WebApplication.Services.Email.Impl;
using Fotron.WebApplication.Services.OAuth.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Linq;
using Fotron.WebApplication.Services.HostedServices;
using Microsoft.Extensions.Hosting;

namespace Fotron.WebApplication {

	public partial class Startup {

		public IServiceProvider ConfigureServices(IServiceCollection services) {

			// app config
			services.AddSingleton(_environment);
			services.AddSingleton(_configuration);
			services.AddSingleton(_appConfig);

			// logger
			services.AddSingleton(LogManager.LogFactory);

			// swagger
			if (!_environment.IsProduction()) {
				services.AddSwaggerGen(opts => {
					opts.SwaggerDoc("api", new Swashbuckle.AspNetCore.Swagger.Info() {
						Title = "API",
						Version = "latest",
					});
					opts.CustomSchemaIds((type) => type.FullName);
					opts.IncludeXmlComments(System.IO.Path.Combine(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath, "Fotron.WebApplication.xml"));
					opts.OperationFilter<Swagger.JWTHeaderParameter>();
					opts.OperationFilter<Swagger.DefaultErrorResponse>();
					opts.DocumentFilter<Swagger.EnumDescription>();
				});
			}

			// db
			services.AddDbContext<ApplicationDbContext>(opts => {
				opts.UseMySql(_appConfig.ConnectionStrings.Default, myopts => {
					myopts.UseRelationalNulls(true);
				});
			});

			// identity
			var idbld = services
				.AddIdentityCore<User>(opts => {
					opts.SignIn.RequireConfirmedEmail = true;
					opts.User.RequireUniqueEmail = true;

					opts.Password.RequireDigit = false;
					opts.Password.RequiredLength = ValidationRules.PasswordMinLength;
					opts.Password.RequireNonAlphanumeric = false;
					opts.Password.RequireUppercase = false;
					opts.Password.RequireLowercase = false;
					opts.Password.RequiredUniqueChars = 1;

					opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
					opts.Lockout.MaxFailedAccessAttempts = 5;
					opts.Lockout.AllowedForNewUsers = true;
				})
			;
			idbld = new IdentityBuilder(idbld.UserType, typeof(Role), services);
			idbld
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddSignInManager<SignInManager<User>>()
				.AddUserManager<Core.UserAccount.GmUserManager>()
				.AddDefaultTokenProviders()
			;

			// auth
			services
				.AddAuthentication(opts => {
					opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
					opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					opts.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
					opts.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(opts => {
					opts.RequireHttpsMetadata = _environment.IsProduction();
					opts.SaveToken = false;
					opts.Events = JWT.AddEvents();
					opts.TokenValidationParameters = JWT.ValidationParameters(_appConfig);
				})
			;

			// authorization
			services.AddAuthorization(opts => {

				// jwt audience
				foreach (var v in (JwtAudience[]) Enum.GetValues(typeof(JwtAudience))) {
					var audSett = _appConfig.Auth.Jwt.Audiences.FirstOrDefault(_ => _.Audience == v.ToString());
					if (audSett != null) {
						opts.AddPolicy(
							Policy.JWTAudienceTemplate + v.ToString(),
							policy => policy.AddRequirements(new RequireJWTAudience(v))
						);
					}
				}

				// jwt area
				foreach (var v in (JwtArea[]) Enum.GetValues(typeof(JwtArea))) {
					opts.AddPolicy(
						Policy.JWTAreaTemplate + v.ToString(),
						policy => policy.AddRequirements(new RequireJWTArea(v))
					);
				}

				// access rights
				foreach (var ar in (AccessRights[]) Enum.GetValues(typeof(AccessRights))) {
					opts.AddPolicy(
						Policy.AccessRightsTemplate + ar.ToString(), 
						policy => policy.AddRequirements(new Core.Policies.RequireAccessRights(ar))
					);
				}
			});

			services.AddSingleton<IAuthorizationHandler, RequireJWTAudience.Handler>();
			services.AddSingleton<IAuthorizationHandler, RequireJWTArea.Handler>();
			services.AddSingleton<IAuthorizationHandler, RequireAccessRights.Handler>();
			services.AddScoped<GoogleProvider>();

			// tokens
			services.Configure<DataProtectionTokenProviderOptions>(opts => {
				opts.Name = "Default";
				opts.TokenLifespan = TimeSpan.FromHours(24);
			});

			// mvc
			services
				.AddMvc(opts => {
					opts.RespectBrowserAcceptHeader = false;
					opts.ReturnHttpNotAcceptable = false;
					opts.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerInputFormatter>();
					opts.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerOutputFormatter>();
				})
				.AddJsonOptions(options => {
					options.SerializerSettings.ContractResolver = Json.CamelCaseSettings.ContractResolver;
				})
			;
			services.AddCors();

			// http context
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			// notifications
			services.AddSingleton<IEmailSender, MailGunSender>();

			// kyc
			//if (_environment.IsProduction()) {
			//	services.AddScoped<IKycProvider>(fac => {
			//		return new ShuftiProKycProvider(opts => {
			//			opts.ClientId = _appConfig.Services.ShuftiPro.ClientId;
			//			opts.ClientSecret = _appConfig.Services.ShuftiPro.ClientSecret;
			//		}, LogManager.LogFactory);
			//	});
			//}
			//else {
			//	services.AddScoped<IKycProvider, DebugKycProvider>();
			//}


			// tron reader
			services.AddSingleton<ITronReader, TronReader>();
			services.AddSingleton<ITronWriter, TronWriter>();

            // workers
		    services.AddSingleton<IHostedService, TokenPriceObserver>();
            services.AddSingleton<IHostedService, TokenStatisticsHarvester>();
		    services.AddSingleton<IHostedService, MaxGasPriceUpdater>();

            services.AddSingleton(new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); }).CreateMapper());

            return services.BuildServiceProvider();
		}

		public void RunServices() {
			var logger = LogManager.LogFactory.GetCurrentClassLogger();
			logger.Info("Run services");
		}

		public void StopServices() {
			var logger = LogManager.LogFactory.GetCurrentClassLogger();
			logger.Info("Stop services");
			logger.Info("Services stopped");
		}
	}
}

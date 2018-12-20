namespace Fotron.Common {

	public sealed class AppConfig {

		public ConnectionStringsSection ConnectionStrings { get; set; } = new ConnectionStringsSection();
		public class ConnectionStringsSection {
			public string Default { get; set; } = "";
        }

		// ---

		public AppsSection Apps { get; set; } = new AppsSection();

		public class AppsSection {

			public string[] AdminEmails { get; set; } = new string[]{};
			public string RelativeApiPath { get; set; } = "/";
			public CabinetSection Cabinet { get; set; } = new CabinetSection();
			public DashboardSection Dashboard { get; set; } = new DashboardSection();

			// ---

			public class CabinetSection : BaseAppSection {

				public string RouteEmailTaken { get; set; } = "";
				public string RouteOAuthTfaPage { get; set; } = "";
				public string RouteOAuthAuthorized { get; set; } = "";
			}

			public class DashboardSection : BaseAppSection {
			}

			public abstract class BaseAppSection {

				public string Url { get; set; } = "/";
			}
		}

		// ---

		public AuthSection Auth { get; set; } = new AuthSection();
		public class AuthSection {

			public JwtSection Jwt { get; set; } = new JwtSection();
			public class JwtSection {

				public string Issuer { get; set; } = "";
				public string Secret { get; set; } = "";
				public AudienceSection[] Audiences { get; set; } = new AudienceSection[0];

				public class AudienceSection {
					public string Audience { get; set; } = "";
					public long ExpirationSec { get; set; } = 1800;
				}
			}

			public string TwoFactorIssuer { get; set; } = "fotron.io";

			public GoogleSection Google { get; set; } = new GoogleSection();
			public class GoogleSection {
				public string ClientId { get; set; } = "";
				public string ClientSecret { get; set; } = "";
			}

			//public ZendeskSsoSection ZendeskSso { get; set; } = new ZendeskSsoSection();
			//public class ZendeskSsoSection {
			//	public string JwtSecret { get; set; } = "";
			//}
		}

		// ---

		public ServicesSection Services { get; set; } = new ServicesSection();
		public class ServicesSection {

			public MailGunSection MailGun { get; set; } = new MailGunSection();
			public class MailGunSection {

				public string Url { get; set; } = "";
				public string DomainName { get; set; } = "";
				public string Key { get; set; } = "";
				public string Sender { get; set; } = "";
			}

			//public ShuftiProSection ShuftiPro { get; set; } = new ShuftiProSection();
			//public class ShuftiProSection {
			
			//	public string ClientId { get; set; } = "";
			//	public string ClientSecret { get; set; } = "";
			//	public string CallbackSecret { get; set; } = "";
			//}


			public TronSection Tron { get; set; } = new TronSection();
			public class TronSection {

                public string FotronCoreAddress { get; set; } = "";
                //public string SetMaxGasPriceFunctionName { get; set; } = "";

                //public string FotronContractAbi { get; set; } = "";
                //public string FotronCoreAbi { get; set; } = "";

                public string TokenPriceFunctionName { get; set; } = "";
			    public string TokenBuyCountFunctionName { get; set; } = "";
			    public string TokenSellCountFunctionName { get; set; } = "";
			    public string BonusPerShareFunctionName { get; set; } = "";
			    public string VolumeEthFunctionName { get; set; } = "";
			    public string VolumeTokenFunctionName { get; set; } = "";

				public string Provider { get; set; } = "";
                //public string GasPriceUrl { get; set; } = "";
                //public string ManagerPrivateKey { get; set; } = "";
            }

			//public WorkersSection Workers { get; set; } = new WorkersSection();
			//public class WorkersSection {
			//}
		}

	}
}

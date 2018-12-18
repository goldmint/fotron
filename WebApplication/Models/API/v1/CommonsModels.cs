using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fotron.WebApplication.Models.API.v1.CommonsModels {

	public class GoldRateView {

		/// <summary>
		/// USD amount per gold ounce
		/// </summary>
		[Required]
		public double Rate { get; set; }
	}

	// ---

	public class TransparencyModel : BasePagerModel {

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<TransparencyModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			return v.Validate(this);
		}
	}

	public class TransparencyView : BasePagerView<TransparencyViewItem> {

		/// <summary>
		/// Current transparency stat
		/// </summary>
		[Required]
		public TransparencyViewStat Stat { get; set; }

	}

	public class TransparencyViewItem {

		/// <summary>
		/// Amount
		/// </summary>
		[Required]
		public string Amount { get; set; }

		/// <summary>
		/// Link to document
		/// </summary>
		[Required]
		public string Link { get; set; }

		/// <summary>
		/// Comment
		/// </summary>
		[Required]
		public string Comment { get; set; }

		/// <summary>
		/// Unixtime
		/// </summary>
		[Required]
		public long Date { get; set; }
	}

	public class TransparencyViewStat {

		/// <summary>
		/// Assets array
		/// </summary>
		[Required]
		public TransparencyViewStatItem[] Assets { get; set; }

		/// <summary>
		/// Liabilities array
		/// </summary>
		[Required]
		public TransparencyViewStatItem[] Bonds { get; set; }

		/// <summary>
		/// Fiat data array
		/// </summary>
		[Required]
		public TransparencyViewStatItem[] Fiat { get; set; }

		/// <summary>
		/// Physical gold data array
		/// </summary>
		[Required]
		public TransparencyViewStatItem[] Gold { get; set; }

		/// <summary>
		/// Total oz
		/// </summary>
		[Required]
		public string TotalOz { get; set; }

		/// <summary>
		/// Total USD
		/// </summary>
		[Required]
		public string TotalUsd { get; set; }

		/// <summary>
		/// Data provided time (unix, optional)
		/// </summary>
		public long? DataTimestamp { get; set; }

		/// <summary>
		/// Audit time (unix, optional)
		/// </summary>
		public long? AuditTimestamp { get; set; }
	}

	public class TransparencyViewStatItem {

		/// <summary>
		/// Item key
		/// </summary>
		[Required]
		public string K { get; set; }

		/// <summary>
		/// Item value
		/// </summary>
		[Required]
		public string V { get; set; }
	}

	// ---

	public class FeesView {

		/// <summary>
		/// Fiat currencies
		/// </summary>
		[Required]
		public FeesViewCurrency[] Fiat { get; set; }

		/// <summary>
		/// Cryptoassets
		/// </summary>
		[Required]
		public FeesViewCurrency[] Crypto { get; set; }
	}

	public class FeesViewCurrency {

		/// <summary>
		/// Currency name: USD, EUR etc.
		/// </summary>
		[Required]
		public string Name { get; set; }
		
		/// <summary>
		/// Currency methods
		/// </summary>
		[Required]
		public FeesViewMethod[] Methods { get; set; }
	}

	public class FeesViewMethod {

		/// <summary>
		/// Method name: VISA, MC etc.
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// Deposit data
		/// </summary>
		[Required]
		public string Deposit { get; set; }

		/// <summary>
		/// Withdraw data
		/// </summary>
		[Required]
		public string Withdraw { get; set; }
	}

	// ---

	public class StatusView {

		/// <summary>
		/// Trading status
		/// </summary>
		[Required]
		public StatusViewTrading Trading { get; set; }

		/// <summary>
		/// Trading limits
		/// </summary>
		[Required]
		public StatusViewLimits Limits { get; set; }
	}

	public class StatusViewTrading {
		
		/// <summary>
		/// Overall trading status
		/// </summary>
		[Required]
		public bool EthAllowed { get; set; }

		/// <summary>
		/// Credit card trading status
		/// </summary>
		[Required]
		public bool CreditCardBuyingAllowed { get; set; }
		
		/// <summary>
		/// Credit card trading status
		/// </summary>
		[Required]
		public bool CreditCardSellingAllowed { get; set; }
	}

	public class StatusViewLimits {

		/// <summary>
		/// Ethereum limits
		/// </summary>
		[Required]
		public Method Eth { get; set; }

		/// <summary>
		/// Credit card
		/// </summary>
		[Required]
		public Method CreditCardUsd { get; set; }

		// ---

		public class Method {

			/// <summary>
			/// Deposit (buying gold)
			/// </summary>
			[Required]
			public MinMax Deposit { get; set; }

			/// <summary>
			/// Withdraw (selling gold)
			/// </summary>
			[Required]
			public MinMax Withdraw { get; set; }
		}

		public class MinMax {

			/// <summary>
			/// Minimal amount
			/// </summary>
			[Required]
			public object Min { get; set; }

			/// <summary>
			/// Maximal amount
			/// </summary>
			[Required]
			public object Max { get; set; }

			/// <summary>
			/// Account limit amount
			/// </summary>
			[Required]
			public object AccountMax { get; set; }

			/// <summary>
			/// Account limit usage amount
			/// </summary>
			[Required]
			public object AccountUsed { get; set; }
		}
	}
}

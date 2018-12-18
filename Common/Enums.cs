namespace Fotron.Common {

	#region Auth

	public enum Locale {

		En = 1,
		Ru,
	}

	public enum LoginProvider {

		Google = 1,
	}

	public enum JwtAudience {

		/// <summary>
		/// App access
		/// </summary>
		Cabinet = 1,

	    /// <summary>
	    /// Dashboard access
	    /// </summary>
	    Dashboard,
    }

	public enum JwtArea {

		/// <summary>
		/// Authorized area
		/// </summary>
		Authorized = 1,

		/// <summary>
		/// TFA area
		/// </summary>
		Tfa,

		/// <summary>
		/// OAuth area
		/// </summary>
		OAuth,

		/// <summary>
		/// User registration
		/// </summary>
		Registration,

		/// <summary>
		/// User password restoration
		/// </summary>
		RestorePassword,

		/// <summary>
		/// DPA awaiting area
		/// </summary>
		Dpa,
	}

	public enum AccessRights : long {

		/// <summary>
		/// App client
		/// </summary>
		Client = 0x1L,

		/// <summary>
		/// Dashboard: general access, read access
		/// </summary>
		DashboardReadAccess = 0x2L,

		/// <summary>
		/// App client - extra access
		/// </summary>
		ClientExtraAccess = 0x4L,

		// ---

		/// <summary>
		/// Dashboard: buy requests write access
		/// </summary>
		BuyRequestsWriteAccess = 0x2000000L,

		/// <summary>
		/// Dashboard: sell requests write access
		/// </summary>
		SellRequestsWriteAccess = 0x4000000L,

		/// <summary>
		/// Dashboard: promo codes access
		/// </summary>
		PromoCodesWriteAccess = 0x8000000L,

		/// <summary>
		/// Dashboard: user list write access
		/// </summary>
		UsersWriteAccess = 0x10000000L,

		/// <summary>
		/// Dashboard: countries tab write access
		/// </summary>
		CountriesWriteAccess = 0x20000000L,

		/// <summary>
		/// Dashboard: transparency tab write access
		/// </summary>
		TransparencyWriteAccess = 0x40000000L,

		/// <summary>
		/// Critical functions access
		/// </summary>
		Owner = 0x80000000L,
	}

	public enum UserTier {

		Tier0 = 0,
		Tier1 = 1,
		Tier2 = 2,
	}

	#endregion


    
	#region Ethereum blockchain



	public enum EthereumOperationType {

		/// <summary>
		/// Transfer GOLD from HW to client address
		/// </summary>
		TransferGoldFromHw = 1,

		/// <summary>
		/// Call contract for request processing (ETH)
		/// </summary>
		ContractProcessBuyRequestEth,

		/// <summary>
		/// Call contract for request cancellation
		/// </summary>
		ContractCancelBuyRequest,

		/// <summary>
		/// Call contract for request processing (ETH)
		/// </summary>
		ContractProcessSellRequestEth,

		/// <summary>
		/// Call contract for request cancellation
		/// </summary>
		ContractCancelSellRequest,

		/// <summary>
		/// Call contract for request processing (fiat)
		/// </summary>
		ContractProcessBuyRequestFiat,

		/// <summary>
		/// Call contract for request processing (fiat)
		/// </summary>
		ContractProcessSellRequestFiat,

		/// <summary>
		/// Transfer ether to the specified address
		/// </summary>
		SendBuyingSupportEther,
	}

	public enum EthereumOperationStatus {

		/// <summary>
		/// Enqueued
		/// </summary>
		Initial = 1,

		/// <summary>
		/// Prepared for processing
		/// </summary>
		Prepared,

		/// <summary>
		/// Sending request to blockchain
		/// </summary>
		BlockchainInit,

		/// <summary>
		/// Waiting confirmation from blockchain
		/// </summary>
		BlockchainConfirm,

		/// <summary>
		/// Success
		/// </summary>
		Success,

		/// <summary>
		/// Failed
		/// </summary>
		Failed,
	}

	public enum EthTransactionStatus {

		/// <summary>
		/// Unconfirmed status, still outside of any block
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Transaction confirmed
		/// </summary>
		Success,

		/// <summary>
		/// Transaction cancelled or failed
		/// </summary>
		Failed,
	}

	#endregion

	#region User

	public enum UserOpLogStatus {

		/// <summary>
		/// Operation is pending
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Operation succesfully completed
		/// </summary>
		Completed,

		/// <summary>
		/// Operation is failed
		/// </summary>
		Failed,
	}

	public enum UserFinHistoryType {

		/// <summary>
		/// Gold purchase
		/// </summary>
		GoldBuy = 1,

		/// <summary>
		/// Gold selling
		/// </summary>
		GoldSell,

		/// <summary>
		/// GOLD transfer from HW
		/// </summary>
		HwTransfer,
	}

	public enum UserFinHistoryStatus {

		/// <summary>
		/// Initially created
		/// </summary>
		Unconfirmed = 1,

		/// <summary>
		/// Manual operation / sent to support team
		/// </summary>
		Manual,

		/// <summary>
		/// Pending
		/// </summary>
		Processing,

		/// <summary>
		/// Completed
		/// </summary>
		Completed,

		/// <summary>
		/// Failed
		/// </summary>
		Failed,
	}

	public enum SignedDocumentType {

		/// <summary>
		/// Terms of service (of sales)
		/// </summary>
		Tos = 1,

		/// <summary>
		/// Data privacy policy (agreement)
		/// </summary>
		Dpa,
	}

	public enum UserActivityType {

		/// <summary>
		/// User logged in
		/// </summary>
		Auth = 1,

		/// <summary>
		/// Password restoration / change
		/// </summary>
		Password,

		/// <summary>
		/// Some setting changed
		/// </summary>
		Settings,

		/// <summary>
		/// Credit card operations
		/// </summary>
		CreditCard,

		/// <summary>
		/// Exchange operations
		/// </summary>
		Exchange,
	}

    public enum PromoCodeUsageType
    {

        /// <summary>
        /// For one user
        /// </summary>
        Single = 1,

        /// <summary>
        /// For multiple users
        /// </summary>
        Multiple
    }


    #endregion

    public enum DbSetting
    {
        RuntimeConfig,
    }


    public enum MutexEntity {

		/// <summary>
		/// Sending a notification (notification-wide)
		/// </summary>
		NotificationSend = 1,

		/// <summary>
		/// Hot wallet operation initiation mutex (user-wide)
		/// </summary>
		UserHwOperation,

		/// <summary>
		/// Processing ethereum opration (operation-wide)
		/// </summary>
		EthOperation,

		/// <summary>
		/// Changing buying request state (request-wide)
		/// </summary>
		GoldBuyingReq,

		/// <summary>
		/// Changing selling request state (request-wide)
		/// </summary>
		GoldSellingReq,

		/// <summary>
		/// Payment check (payment-wide)
		/// </summary>
		CardPaymentCheck,
	}

	public enum NotificationType {

		Email = 1,
	}
}

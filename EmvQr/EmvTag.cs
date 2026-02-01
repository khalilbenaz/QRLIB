namespace EmvQr
{
    /// <summary>
    /// Constants for EMVCo QR code tag identifiers
    /// </summary>
    public static class EmvTag
    {
        // === Merchant Presented Mode (MPM) Tags ===

        /// <summary>
        /// Tag 00: Payload Format Indicator (mandatory) - Identifies the EMV QR code specification version
        /// </summary>
        public const string PayloadFormatIndicator = "00";

        /// <summary>
        /// Tag 01: Point of Initiation Method (mandatory) - 11 for static, 12 for dynamic
        /// </summary>
        public const string PointOfInitiationMethod = "01";

        /// <summary>
        /// Tags 02-51: Merchant Account Information (mandatory, at least one required)
        /// Provides the merchant's account information for payment
        /// </summary>
        public const string MerchantCategoryCode = "52";

        /// <summary>
        /// Tag 53: Transaction Currency (mandatory) - 3-digit ISO 4217 numeric code
        /// </summary>
        public const string TransactionCurrency = "53";

        /// <summary>
        /// Tag 54: Transaction Amount (conditional) - Amount of the transaction
        /// </summary>
        public const string TransactionAmount = "54";

        /// <summary>
        /// Tag 55: Tip or Convenience Indicator (optional) - 01 for tip, 02 for convenience fee fixed, 03 for percentage
        /// </summary>
        public const string TipOrConvenienceIndicator = "55";

        /// <summary>
        /// Tag 56: Value of Convenience Fee Fixed (conditional) - Fixed amount for convenience fee
        /// </summary>
        public const string ValueOfConvenienceFeeFixed = "56";

        /// <summary>
        /// Tag 57: Value of Convenience Fee Percentage (conditional) - Percentage for convenience fee
        /// </summary>
        public const string ValueOfConvenienceFeePercentage = "57";

        /// <summary>
        /// Tag 58: Country Code (mandatory) - 2-letter ISO 3166 alpha-2 code
        /// </summary>
        public const string CountryCode = "58";

        /// <summary>
        /// Tag 59: Merchant Name (mandatory) - Name of the merchant
        /// </summary>
        public const string MerchantName = "59";

        /// <summary>
        /// Tag 60: Merchant City (mandatory) - City where the merchant is located
        /// </summary>
        public const string MerchantCity = "60";

        /// <summary>
        /// Tag 61: Postal Code (optional) - Postal code of the merchant
        /// </summary>
        public const string PostalCode = "61";

        /// <summary>
        /// Tag 62: Additional Data Field Template (optional) - Additional merchant data in nested format
        /// </summary>
        public const string AdditionalDataFieldTemplate = "62";

        /// <summary>
        /// Tag 63: CRC (mandatory) - Checksum for data integrity verification
        /// </summary>
        public const string CRC = "63";

        // === Ranges ===

        /// <summary>
        /// Start of Merchant Account Information tag range (Tag 02)
        /// </summary>
        public const int MerchantAccountInfoStart = 2;

        /// <summary>
        /// End of Merchant Account Information tag range (Tag 51)
        /// </summary>
        public const int MerchantAccountInfoEnd = 51;

        // === Additional Data Field Sub-Tags (for Tag 62) ===

        /// <summary>
        /// Sub-tag 01: Bill Number
        /// </summary>
        public const string BillNumber = "01";

        /// <summary>
        /// Sub-tag 02: Mobile Number
        /// </summary>
        public const string MobileNumber = "02";

        /// <summary>
        /// Sub-tag 03: Store Label
        /// </summary>
        public const string StoreLabel = "03";

        /// <summary>
        /// Sub-tag 04: Loyalty Number
        /// </summary>
        public const string LoyaltyNumber = "04";

        /// <summary>
        /// Sub-tag 05: Reference Label
        /// </summary>
        public const string ReferenceLabel = "05";

        /// <summary>
        /// Sub-tag 06: Customer Label
        /// </summary>
        public const string CustomerLabel = "06";

        /// <summary>
        /// Sub-tag 07: Terminal Label
        /// </summary>
        public const string TerminalLabel = "07";

        /// <summary>
        /// Sub-tag 08: Purpose of Transaction
        /// </summary>
        public const string PurposeOfTransaction = "08";

        /// <summary>
        /// Sub-tag 09: Additional Consumer Data Request
        /// </summary>
        public const string AdditionalConsumerDataRequest = "09";

        // === Merchant Account Information Sub-Tags ===

        /// <summary>
        /// Sub-tag 00: Globally Unique Identifier (GUID) - Used in Merchant Account Information
        /// </summary>
        public const string GloballyUniqueIdentifier = "00";

        /// <summary>
        /// Sub-tag 01: Payment Network Specific - Used in Merchant Account Information
        /// </summary>
        public const string PaymentNetworkSpecific = "01";
    }
}

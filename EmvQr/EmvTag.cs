namespace EmvQr
{
    public static class EmvTag
    {
        public const string PayloadFormatIndicator = "00";
        public const string PointOfInitiationMethod = "01";
        // Merchant Account Information 02-51
        public const string MerchantCategoryCode = "52";
        public const string TransactionCurrency = "53";
        public const string TransactionAmount = "54";
        public const string TipOrConvenienceIndicator = "55";
        public const string ValueOfConvenienceFeeFixed = "56";
        public const string ValueOfConvenienceFeePercentage = "57";
        public const string CountryCode = "58";
        public const string MerchantName = "59";
        public const string MerchantCity = "60";
        public const string PostalCode = "61";
        public const string AdditionalDataFieldTemplate = "62";
        public const string CRC = "63";
        
        // Merchant Account Info Range
        public const int MerchantAccountInfoStart = 2;
        public const int MerchantAccountInfoEnd = 51;

        // Additional Data Field IDs
        public const string BillNumber = "01";
        public const string MobileNumber = "02";
        public const string StoreLabel = "03";
        public const string LoyaltyNumber = "04";
        public const string ReferenceLabel = "05";
        public const string CustomerLabel = "06";
        public const string TerminalLabel = "07";
        public const string PurposeOfTransaction = "08";
        public const string AdditionalConsumerDataRequest = "09";

        // Common Sub-Tags
        public const string GloballyUniqueIdentifier = "00";
        public const string PaymentNetworkSpecific = "01";
    }
}

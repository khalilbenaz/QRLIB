namespace EmvQr
{
    public class EmvValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new List<string>();
    }

    public static class EmvValidator
    {
        public static EmvValidationResult Validate(EmvQrCode qr)
        {
            var result = new EmvValidationResult();

            // Mandatory Tag 00: Payload Format Indicator
            if (qr.Get(EmvTag.PayloadFormatIndicator) == null)
                result.Errors.Add($"Missing Mandatory Tag '{EmvTag.PayloadFormatIndicator}': Payload Format Indicator.");

            // Mandatory Tag 01: Point of Initiation Method
            if (qr.Get(EmvTag.PointOfInitiationMethod) == null)
                result.Errors.Add($"Missing Mandatory Tag '{EmvTag.PointOfInitiationMethod}': Point of Initiation Method.");

            // Mandatory Tag 52: Merchant Category Code
            if (qr.Get(EmvTag.MerchantCategoryCode) == null)
                result.Errors.Add($"Missing Mandatory Tag '{EmvTag.MerchantCategoryCode}': Merchant Category Code.");

            // Mandatory Tag 53: Transaction Currency
            if (qr.Get(EmvTag.TransactionCurrency) == null)
                result.Errors.Add($"Missing Mandatory Tag '{EmvTag.TransactionCurrency}': Transaction Currency.");

            // Tag 54 (Amount) is conditional depending on context, but usually recommended. We skip mandatory check for dynamic QRs.

            // Mandatory Tag 58: Country Code
            if (qr.Get(EmvTag.CountryCode) == null)
                result.Errors.Add($"Missing Mandatory Tag '{EmvTag.CountryCode}': Country Code.");

            // Mandatory Tag 59: Merchant Name
            if (qr.Get(EmvTag.MerchantName) == null)
                result.Errors.Add($"Missing Mandatory Tag '{EmvTag.MerchantName}': Merchant Name.");

            // Mandatory Tag 60: Merchant City
            if (qr.Get(EmvTag.MerchantCity) == null)
                result.Errors.Add($"Missing Mandatory Tag '{EmvTag.MerchantCity}': Merchant City.");

            // Check Tag 2-51: At least one Merchant Account Information must be present
            bool hasMerchantInfo = qr.DataObjects.Any(d => 
                int.TryParse(d.Tag, out int tagNum) && 
                tagNum >= EmvTag.MerchantAccountInfoStart && 
                tagNum <= EmvTag.MerchantAccountInfoEnd);

            if (!hasMerchantInfo)
                result.Errors.Add("Missing Merchant Account Information (Tags 02-51). At least one must be defined.");

            return result;
        }
    }
}

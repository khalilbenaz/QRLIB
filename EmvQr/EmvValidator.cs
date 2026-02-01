using EmvQr.Standards;
using System.Text.RegularExpressions;

namespace EmvQr
{
    public class EmvValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
    }

    public static class EmvValidator
    {
        private static readonly Regex NumericRegex = new Regex(@"^\d+$");
        private static readonly Regex AlphaNumericRegex = new Regex(@"^[a-zA-Z0-9]+$");
        private static readonly Regex AmountRegex = new Regex(@"^\d+(\.\d{1,2})?$");
        private static readonly Regex CountryCodeRegex = new Regex(@"^[A-Z]{2}$");
        private static readonly Regex MerchantCategoryCodeRegex = new Regex(@"^\d{4}$");
        private static readonly Regex CurrencyCodeRegex = new Regex(@"^\d{3}$");
        private static readonly Regex HexRegex = new Regex(@"^[0-9A-Fa-f]+$");

        public static EmvValidationResult Validate(EmvQrCode qr)
        {
            var result = new EmvValidationResult();

            // Mandatory Tag 00: Payload Format Indicator
            ValidateMandatoryTag(qr, EmvTag.PayloadFormatIndicator, "Payload Format Indicator", result,
                value => value.Length >= 1 && value.Length <= 2 && NumericRegex.IsMatch(value));

            // Mandatory Tag 01: Point of Initiation Method
            ValidateMandatoryTag(qr, EmvTag.PointOfInitiationMethod, "Point of Initiation Method", result,
                value => (value == "11" || value == "12") && value.Length == 2);

            // Mandatory Tag 52: Merchant Category Code
            ValidateMandatoryTag(qr, EmvTag.MerchantCategoryCode, "Merchant Category Code", result,
                value => MerchantCategoryCodeRegex.IsMatch(value));

            // Mandatory Tag 53: Transaction Currency
            ValidateMandatoryTag(qr, EmvTag.TransactionCurrency, "Transaction Currency", result,
                value => CurrencyCodeRegex.IsMatch(value) && Currencies.IsValid(value));

            // Conditional Tag 54: Transaction Amount
            ValidateOptionalTag(qr, EmvTag.TransactionAmount, "Transaction Amount", result,
                value => AmountRegex.IsMatch(value));

            // Mandatory Tag 58: Country Code
            ValidateMandatoryTag(qr, EmvTag.CountryCode, "Country Code", result,
                value => CountryCodeRegex.IsMatch(value) && Countries.IsValid(value));

            // Mandatory Tag 59: Merchant Name
            ValidateMandatoryTag(qr, EmvTag.MerchantName, "Merchant Name", result,
                value => !string.IsNullOrWhiteSpace(value) && value.Length >= 1 && value.Length <= 25);

            // Mandatory Tag 60: Merchant City
            ValidateMandatoryTag(qr, EmvTag.MerchantCity, "Merchant City", result,
                value => !string.IsNullOrWhiteSpace(value) && value.Length >= 1 && value.Length <= 15);

            // Optional Tag 61: Postal Code
            ValidateOptionalTag(qr, EmvTag.PostalCode, "Postal Code", result,
                value => !string.IsNullOrWhiteSpace(value) && value.Length <= 10);

            // Check Tag 2-51: At least one Merchant Account Information must be present
            bool hasMerchantInfo = qr.DataObjects.Any(d =>
                int.TryParse(d.Tag, out int tagNum) &&
                tagNum >= EmvTag.MerchantAccountInfoStart &&
                tagNum <= EmvTag.MerchantAccountInfoEnd);

            if (!hasMerchantInfo)
                result.Errors.Add("Missing Merchant Account Information (Tags 02-51). At least one must be defined.");
            else
            {
                // Validate all merchant account information tags
                var merchantInfoTags = qr.DataObjects.Where(d =>
                    int.TryParse(d.Tag, out int tagNum) &&
                    tagNum >= EmvTag.MerchantAccountInfoStart &&
                    tagNum <= EmvTag.MerchantAccountInfoEnd);

                foreach (var tag in merchantInfoTags)
                {
                    ValidateTagLength(tag.Value, tag.Tag, "Merchant Account Information", 1, 99, result);
                }
            }

            // Validate Additional Data Field Template (Tag 62) if present
            var additionalData = qr.Get(EmvTag.AdditionalDataFieldTemplate);
            if (additionalData != null)
            {
                if (additionalData.IsNested)
                {
                    foreach (var nested in additionalData.NestedData)
                    {
                        ValidateAdditionalDataField(nested.Tag, nested.Value, result);
                    }
                }
                else
                {
                    result.Warnings.Add($"Tag '{EmvTag.AdditionalDataFieldTemplate}' should contain nested data.");
                }
            }

            // Validate CRC (Tag 63) if present
            var crc = qr.Get(EmvTag.CRC);
            if (crc != null)
            {
                ValidateTagLength(crc.Value, EmvTag.CRC, "CRC", 4, 4, result,
                    value => HexRegex.IsMatch(value));
            }
            else
            {
                result.Warnings.Add($"Tag '{EmvTag.CRC}' is recommended for data integrity check.");
            }

            return result;
        }

        private static void ValidateMandatoryTag(EmvQrCode qr, string tag, string description, EmvValidationResult result, Func<string, bool>? validator = null)
        {
            var data = qr.Get(tag);
            if (data == null)
            {
                result.Errors.Add($"Missing Mandatory Tag '{tag}': {description}.");
                return;
            }

            if (string.IsNullOrWhiteSpace(data.Value))
            {
                result.Errors.Add($"Tag '{tag}' ({description}) has empty value.");
                return;
            }

            if (validator != null && !validator(data.Value))
            {
                result.Errors.Add($"Tag '{tag}' ({description}) has invalid format: '{data.Value}'.");
            }
        }

        private static void ValidateOptionalTag(EmvQrCode qr, string tag, string description, EmvValidationResult result, Func<string, bool>? validator = null)
        {
            var data = qr.Get(tag);
            if (data == null)
                return;

            if (string.IsNullOrWhiteSpace(data.Value))
            {
                result.Errors.Add($"Tag '{tag}' ({description}) has empty value.");
                return;
            }

            if (validator != null && !validator(data.Value))
            {
                result.Errors.Add($"Tag '{tag}' ({description}) has invalid format: '{data.Value}'.");
            }
        }

        private static void ValidateTagLength(string value, string tag, string description, int minLength, int maxLength, EmvValidationResult result, Func<string, bool>? validator = null)
        {
            if (value.Length < minLength || value.Length > maxLength)
            {
                result.Errors.Add($"Tag '{tag}' ({description}) has invalid length: '{value}' (must be between {minLength} and {maxLength} characters).");
            }

            if (validator != null && !validator(value))
            {
                result.Errors.Add($"Tag '{tag}' ({description}) has invalid format: '{value}'.");
            }
        }

        private static void ValidateAdditionalDataField(string tag, string value, EmvValidationResult result)
        {
            // Check if tag is a valid additional data field ID
            if (!int.TryParse(tag, out int tagNum) || tagNum < 0 || tagNum > 99)
            {
                result.Errors.Add($"Invalid additional data field tag: '{tag}' (must be 00-99).");
                return;
            }

            // Validate length (should be between 1 and 99 characters)
            if (value.Length < 1 || value.Length > 99)
            {
                result.Errors.Add($"Additional data field '{tag}' has invalid length: '{value}' (must be between 1 and 99 characters).");
            }
        }

        /// <summary>
        /// Validates and throws an exception if the QR code is not valid
        /// </summary>
        public static void ValidateAndThrow(EmvQrCode qr)
        {
            var result = Validate(qr);
            if (!result.IsValid)
            {
                throw new EmvValidationException(result);
            }
        }
    }
}

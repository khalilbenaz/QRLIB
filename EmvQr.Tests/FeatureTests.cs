using Xunit;
using EmvQr;

namespace EmvQr.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void Validator_Detects_Missing_Fields()
        {
            // Empty QR
            var qr = new EmvQrCode(); 
            var result = EmvValidator.Validate(qr);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Payload Format Indicator"));
            Assert.Contains(result.Errors, e => e.Contains("Merchant Category Code"));
            Assert.Contains(result.Errors, e => e.Contains("Transaction Currency"));
            Assert.Contains(result.Errors, e => e.Contains("Country Code"));
            Assert.Contains(result.Errors, e => e.Contains("Merchant Name"));
            Assert.Contains(result.Errors, e => e.Contains("Merchant City"));
            Assert.Contains(result.Errors, e => e.Contains("Merchant Account Information"));
        }

        [Fact]
        public void Validator_Passes_Valid_QR()
        {
            var builder = new EmvBuilder()
                .SetPayloadFormatIndicator("01")
                .SetPointOfInitiationMethod(false)
                .SetMerchantCategoryCode("5411")
                .SetTransactionCurrency("978")
                .SetTransactionAmount(10.00)
                .SetCountryCode("FR")
                .SetMerchantName("Boulangerie")
                .SetMerchantCity("Lyon")
                .AddMerchantAccountInformation("26", "GUID_VISA", "DATA_123");

            var qrRaw = builder.Build();
            var qr = EmvParser.Parse(qrRaw);
            
            var result = EmvValidator.Validate(qr);
            
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }

    public class AdvancedBuilderTests
    {
        [Fact]
        public void Builder_Creates_Nested_Merchant_Account_Info()
        {
            var builder = new EmvBuilder();
            // ID 26 (e.g., Visa) with nested GUID (00) and Data (01)
            builder.AddMerchantAccountInformation("26", "A000000003", "12345678");
            
            string qrRaw = builder.Build();
            var qr = EmvParser.Parse(qrRaw);
            
            // Note: The Parser logic in v1.0.0 was basic. 
            // We need to verify if the Parser correctly treats 26-51 as POTENTIALLY nested or simple string.
            // Currently, our EmvParser.Parse logic only auto-detects nested for Tag 62.
            // For Tag 26, it will read it as a simple Value. 
            // We should check that the Value contains the TLV structure.
            
            var accountInfo = qr.Get("26");
            Assert.NotNull(accountInfo);
            
            // Should contain "00" (Len 10) "A000000003" + "01" (Len 08) "12345678"
            // "0010A000000003010812345678"
            string expectedValue = "0010A000000003010812345678";
            Assert.Equal(expectedValue, accountInfo.Value);
        }
    }
}

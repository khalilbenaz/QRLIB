using Xunit;
using EmvQr;

namespace EmvQr.Tests
{
    public class CoreTests
    {
        [Fact]
        public void Crc16_Calculation_Is_Correct()
        {
            // Example from EMVCo spec or known vectors
            // "000201010211" -> CRC is "5163" 
            // 00(Tag) 02(Len) 01(Val) -> "000201"
            // 01(Tag) 02(Len) 11(Val) -> "010211"
            // Combined: 000201010211
            // CRC ID "63", Len "04" -> "6304"
            // Total data to checksum: "0002010102116304"
            
            string data = "0002010102116304";
            string crc = Crc16.Compute(data);
            Assert.Equal("AD0A", crc); // Calculated manually for 0002010102116304
        }

        [Fact]
        public void Builder_Creates_Valid_String()
        {
            var builder = new EmvBuilder()
                .SetPointOfInitiationMethod(false) // 11
                .SetMerchantCategoryCode("1234")
                .SetTransactionCurrency("840")
                .SetTransactionAmount(99.50)
                .SetCountryCode("US")
                .SetMerchantName("Test Merchant")
                .SetMerchantCity("New York");

            string qr = builder.Build();

            Assert.StartsWith("000201", qr); // Payload Format
            Assert.Contains("010211", qr); // Static POI
            Assert.Contains("52041234", qr); // MCC
            Assert.Contains("540599.50", qr); // Amount
            Assert.EndsWith(Crc16.Compute(qr.Substring(0, qr.Length - 4)), qr.Substring(qr.Length - 4));
        }

        [Fact]
        public void Parser_Reads_Correctly()
        {
            string raw = "000201010211520412345802US5913Test Merchant6304D946";
            // Checksum D946 is made up, let's just check parsing logic ignoring CRC validation in Parser itself (Parser just reads)
            
            var qr = EmvParser.Parse(raw);
            
            Assert.Equal("01", qr.Get("00")?.Value);
            Assert.Equal("11", qr.Get("01")?.Value);
            Assert.Equal("1234", qr.Get("52")?.Value);
            Assert.Equal("US", qr.Get("58")?.Value);
            Assert.Equal("Test Merchant", qr.Get("59")?.Value);
            Assert.Equal("D946", qr.Get("63")?.Value);
        }

        [Fact]
        public void Nested_Additional_Data_Works()
        {
             var builder = new EmvBuilder();
             builder.AddAdditionalData("01", "Bill123");
             builder.AddAdditionalData("07", "Term1");
             
             string qrRaw = builder.Build();
             
             var qr = EmvParser.Parse(qrRaw);
             var additional = qr.Get("62");
             
             Assert.NotNull(additional);
             Assert.True(additional.IsNested);
             Assert.Equal(2, additional.NestedData.Count);
             Assert.Equal("Bill123", additional.NestedData.First(x => x.Tag == "01").Value);
        }
    }
}

using Xunit;
using EmvQr;
using EmvQr.Standards;

namespace EmvQr.Tests
{
    public class StandardsTests
    {
        [Fact]
        public void Builder_Uses_Standards_Correctly()
        {
            var builder = new EmvBuilder()
                .SetTransactionCurrency(Currencies.EUR)
                .SetCountryCode(Countries.FR)
                .SetMerchantCategoryCode(MerchantCategoryCodes.GroceryStoresSupermarkets);

            string qr = builder.Build();
            
            // EUR = 978, FR = FR, Grocery = 5411
            Assert.Contains("5303978", qr);
            Assert.Contains("5802FR", qr);
            Assert.Contains("52045411", qr);
        }
    }
}

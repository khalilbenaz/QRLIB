using EmvQr.Standards;

namespace EmvQr
{
    public class EmvBuilder
    {
        private EmvQrCode _qr;

        public EmvBuilder()
        {
            _qr = new EmvQrCode();
            // Default Version 01
            _qr.AddData(EmvTag.PayloadFormatIndicator, "01");
        }

        public EmvBuilder SetPayloadFormatIndicator(string version = "01")
        {
            // Remove existing if present to replace
            var existing = _qr.Get(EmvTag.PayloadFormatIndicator);
            if (existing != null) _qr.DataObjects.Remove(existing);
            
            _qr.AddData(EmvTag.PayloadFormatIndicator, version);
            return this;
        }

        public EmvBuilder SetPointOfInitiationMethod(bool dynamic)
        {
            _qr.AddData(EmvTag.PointOfInitiationMethod, dynamic ? "12" : "11");
            return this;
        }

        public EmvBuilder AddMerchantAccountInformation(string id, string value)
        {
            // ID must be between 02 and 51
            if (int.TryParse(id, out int idNum))
            {
                if (idNum < 2 || idNum > 51)
                    throw new ArgumentOutOfRangeException(nameof(id), "Merchant Account Info ID must be between 02 and 51");
            }
            _qr.AddData(id, value);
            return this;
        }

        /// <summary>
        /// Adds a complex Merchant Account Information (Tags 02-51) containing a GUID and specific data.
        /// Common usage for schemes like Visa, MasterCard, etc.
        /// </summary>
        /// <param name="id">The parent tag ID (02-51)</param>
        /// <param name="guid">The Globally Unique Identifier (Sub-tag 00)</param>
        /// <param name="paymentNetworkSpecific">The network specific data (Sub-tag 01)</param>
        public EmvBuilder AddMerchantAccountInformation(string id, string guid, string paymentNetworkSpecific)
        {
            if (int.TryParse(id, out int idNum))
            {
                if (idNum < 2 || idNum > 51)
                    throw new ArgumentOutOfRangeException(nameof(id), "Merchant Account Info ID must be between 02 and 51");
            }

            var nested = new List<EmvDataObject>
            {
                new EmvDataObject(EmvTag.GloballyUniqueIdentifier, guid),
                new EmvDataObject(EmvTag.PaymentNetworkSpecific, paymentNetworkSpecific)
            };

            _qr.AddNestedData(id, nested);
            return this;
        }

        /// <summary>
        /// Sets the Merchant Category Code (Tag 52).
        /// Use <see cref="MerchantCategoryCodes"/> for standard values.
        /// </summary>
        /// <param name="mcc">The 4-digit MCC.</param>
        public EmvBuilder SetMerchantCategoryCode(string mcc)
        {
            _qr.AddData(EmvTag.MerchantCategoryCode, mcc);
            return this;
        }

        /// <summary>
        /// Sets the Transaction Currency (Tag 53).
        /// Use <see cref="Currencies"/> for standard values (ISO 4217).
        /// </summary>
        /// <param name="currencyCode">The 3-digit numeric currency code.</param>
        public EmvBuilder SetTransactionCurrency(string currencyCode)
        {
            _qr.AddData(EmvTag.TransactionCurrency, currencyCode);
            return this;
        }

        public EmvBuilder SetTransactionAmount(double amount)
        {
            // Basic formatting, no exponential
            _qr.AddData(EmvTag.TransactionAmount, amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            return this;
        }

        /// <summary>
        /// Sets the Country Code (Tag 58).
        /// Use <see cref="Countries"/> for standard values (ISO 3166).
        /// </summary>
        /// <param name="countryCode">The 2-letter country code.</param>
        public EmvBuilder SetCountryCode(string countryCode)
        {
            _qr.AddData(EmvTag.CountryCode, countryCode);
            return this;
        }

        public EmvBuilder SetMerchantName(string name)
        {
            _qr.AddData(EmvTag.MerchantName, name);
            return this;
        }

        public EmvBuilder SetMerchantCity(string city)
        {
            _qr.AddData(EmvTag.MerchantCity, city);
            return this;
        }

        public EmvBuilder AddAdditionalData(string id, string value)
        {
             // Check if ID 62 exists
             var existing = _qr.Get(EmvTag.AdditionalDataFieldTemplate);
             if (existing == null)
             {
                 var list = new List<EmvDataObject>();
                 list.Add(new EmvDataObject(id, value));
                 _qr.AddNestedData(EmvTag.AdditionalDataFieldTemplate, list);
             }
             else
             {
                 if (!existing.IsNested) 
                 {
                     // Convert to nested? Or throw?
                     // For simplicity, we assume if using Builder, we manage it correctly.
                     throw new InvalidOperationException("Additional Data 62 is already set as simple string.");
                 }
                 existing.NestedData.Add(new EmvDataObject(id, value));
                 // Force value update
                 existing.ToString(); 
             }
             return this;
        }

        public string Build()
        {
            return _qr.GeneratePayload();
        }
    }
}

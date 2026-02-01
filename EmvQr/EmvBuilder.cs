using EmvQr.Standards;
using System.Globalization;

namespace EmvQr
{
    /// <summary>
    /// Fluent API for building EMV QR Codes
    /// </summary>
    public class EmvBuilder
    {
        private EmvQrCode _qr;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmvBuilder"/> class
        /// </summary>
        public EmvBuilder()
        {
            _qr = new EmvQrCode();
            // Default Version 01 (required by EMVCo specification)
            _qr.AddData(EmvTag.PayloadFormatIndicator, "01");
        }

        /// <summary>
        /// Sets the Payload Format Indicator (Tag 00). Default is "01"
        /// </summary>
        /// <param name="version">The version string (1-2 characters)</param>
        public EmvBuilder SetPayloadFormatIndicator(string version = "01")
        {
            _qr.UpdateData(EmvTag.PayloadFormatIndicator, version);
            return this;
        }

        /// <summary>
        /// Sets the Point of Initiation Method (Tag 01)
        /// </summary>
        /// <param name="dynamic">True for dynamic QR codes (12), false for static (11)</param>
        public EmvBuilder SetPointOfInitiationMethod(bool dynamic)
        {
            _qr.AddData(EmvTag.PointOfInitiationMethod, dynamic ? "12" : "11");
            return this;
        }

        /// <summary>
        /// Adds Merchant Account Information (Tags 02-51)
        /// </summary>
        /// <param name="id">The tag ID (02-51)</param>
        /// <param name="value">The account information value</param>
        public EmvBuilder AddMerchantAccountInformation(string id, string value)
        {
            if (int.TryParse(id, out int idNum))
            {
                if (idNum < 2 || idNum > 51)
                    throw new ArgumentOutOfRangeException(nameof(id), "Merchant Account Info ID must be between 02 and 51");
            }
            _qr.AddData(id, value);
            return this;
        }

        /// <summary>
        /// Adds complex Merchant Account Information (Tags 02-51) containing a GUID and payment network specific data
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
        /// Sets the Merchant Category Code (Tag 52)
        /// </summary>
        /// <param name="mcc">The 4-digit MCC. Use <see cref="MerchantCategoryCodes"/> for standard values</param>
        public EmvBuilder SetMerchantCategoryCode(string mcc)
        {
            _qr.AddData(EmvTag.MerchantCategoryCode, mcc);
            return this;
        }

        /// <summary>
        /// Sets the Transaction Currency (Tag 53)
        /// </summary>
        /// <param name="currencyCode">The 3-digit numeric currency code. Use <see cref="Currencies"/> for standard values (ISO 4217)</param>
        public EmvBuilder SetTransactionCurrency(string currencyCode)
        {
            _qr.AddData(EmvTag.TransactionCurrency, currencyCode);
            return this;
        }

        /// <summary>
        /// Sets the Transaction Amount (Tag 54)
        /// </summary>
        /// <param name="amount">The transaction amount</param>
        public EmvBuilder SetTransactionAmount(double amount)
        {
            // Format to 2 decimal places
            _qr.AddData(EmvTag.TransactionAmount, amount.ToString("0.00", CultureInfo.InvariantCulture));
            return this;
        }

        /// <summary>
        /// Sets the Transaction Amount (Tag 54)
        /// </summary>
        /// <param name="amount">The transaction amount as string</param>
        public EmvBuilder SetTransactionAmount(string amount)
        {
            _qr.AddData(EmvTag.TransactionAmount, amount);
            return this;
        }

        /// <summary>
        /// Sets the Tip or Convenience Indicator (Tag 55)
        /// </summary>
        /// <param name="indicator">The tip or convenience indicator</param>
        public EmvBuilder SetTipOrConvenienceIndicator(string indicator)
        {
            _qr.AddData(EmvTag.TipOrConvenienceIndicator, indicator);
            return this;
        }

        /// <summary>
        /// Sets the Value of Convenience Fee Fixed (Tag 56)
        /// </summary>
        /// <param name="fee">The fixed convenience fee</param>
        public EmvBuilder SetValueOfConvenienceFeeFixed(double fee)
        {
            _qr.AddData(EmvTag.ValueOfConvenienceFeeFixed, fee.ToString("0.00", CultureInfo.InvariantCulture));
            return this;
        }

        /// <summary>
        /// Sets the Value of Convenience Fee Percentage (Tag 57)
        /// </summary>
        /// <param name="percentage">The convenience fee percentage</param>
        public EmvBuilder SetValueOfConvenienceFeePercentage(double percentage)
        {
            _qr.AddData(EmvTag.ValueOfConvenienceFeePercentage, percentage.ToString("0.00", CultureInfo.InvariantCulture));
            return this;
        }

        /// <summary>
        /// Sets the Country Code (Tag 58)
        /// </summary>
        /// <param name="countryCode">The 2-letter country code. Use <see cref="Countries"/> for standard values (ISO 3166)</param>
        public EmvBuilder SetCountryCode(string countryCode)
        {
            _qr.AddData(EmvTag.CountryCode, countryCode.ToUpper());
            return this;
        }

        /// <summary>
        /// Sets the Merchant Name (Tag 59)
        /// </summary>
        /// <param name="name">The merchant name</param>
        public EmvBuilder SetMerchantName(string name)
        {
            _qr.AddData(EmvTag.MerchantName, name);
            return this;
        }

        /// <summary>
        /// Sets the Merchant City (Tag 60)
        /// </summary>
        /// <param name="city">The merchant city</param>
        public EmvBuilder SetMerchantCity(string city)
        {
            _qr.AddData(EmvTag.MerchantCity, city);
            return this;
        }

        /// <summary>
        /// Sets the Postal Code (Tag 61)
        /// </summary>
        /// <param name="postalCode">The postal code</param>
        public EmvBuilder SetPostalCode(string postalCode)
        {
            _qr.AddData(EmvTag.PostalCode, postalCode);
            return this;
        }

        /// <summary>
        /// Adds additional data to the Additional Data Field Template (Tag 62)
        /// </summary>
        /// <param name="id">The additional data field ID (00-99)</param>
        /// <param name="value">The additional data value</param>
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
                    throw new InvalidOperationException("Additional Data 62 is already set as simple string.");
                }
                existing.NestedData.Add(new EmvDataObject(id, value));
                existing.ToString();
            }
            return this;
        }

        /// <summary>
        /// Adds Bill Number to Additional Data Field (Tag 62, Sub-tag 01)
        /// </summary>
        /// <param name="billNumber">The bill number</param>
        public EmvBuilder SetBillNumber(string billNumber)
        {
            return AddAdditionalData(EmvTag.BillNumber, billNumber);
        }

        /// <summary>
        /// Adds Mobile Number to Additional Data Field (Tag 62, Sub-tag 02)
        /// </summary>
        /// <param name="mobileNumber">The mobile number</param>
        public EmvBuilder SetMobileNumber(string mobileNumber)
        {
            return AddAdditionalData(EmvTag.MobileNumber, mobileNumber);
        }

        /// <summary>
        /// Adds Store Label to Additional Data Field (Tag 62, Sub-tag 03)
        /// </summary>
        /// <param name="storeLabel">The store label</param>
        public EmvBuilder SetStoreLabel(string storeLabel)
        {
            return AddAdditionalData(EmvTag.StoreLabel, storeLabel);
        }

        /// <summary>
        /// Adds Loyalty Number to Additional Data Field (Tag 62, Sub-tag 04)
        /// </summary>
        /// <param name="loyaltyNumber">The loyalty number</param>
        public EmvBuilder SetLoyaltyNumber(string loyaltyNumber)
        {
            return AddAdditionalData(EmvTag.LoyaltyNumber, loyaltyNumber);
        }

        /// <summary>
        /// Adds Reference Label to Additional Data Field (Tag 62, Sub-tag 05)
        /// </summary>
        /// <param name="referenceLabel">The reference label</param>
        public EmvBuilder SetReferenceLabel(string referenceLabel)
        {
            return AddAdditionalData(EmvTag.ReferenceLabel, referenceLabel);
        }

        /// <summary>
        /// Adds Customer Label to Additional Data Field (Tag 62, Sub-tag 06)
        /// </summary>
        /// <param name="customerLabel">The customer label</param>
        public EmvBuilder SetCustomerLabel(string customerLabel)
        {
            return AddAdditionalData(EmvTag.CustomerLabel, customerLabel);
        }

        /// <summary>
        /// Adds Terminal Label to Additional Data Field (Tag 62, Sub-tag 07)
        /// </summary>
        /// <param name="terminalLabel">The terminal label</param>
        public EmvBuilder SetTerminalLabel(string terminalLabel)
        {
            return AddAdditionalData(EmvTag.TerminalLabel, terminalLabel);
        }

        /// <summary>
        /// Adds Purpose of Transaction to Additional Data Field (Tag 62, Sub-tag 08)
        /// </summary>
        /// <param name="purpose">The purpose of transaction</param>
        public EmvBuilder SetPurposeOfTransaction(string purpose)
        {
            return AddAdditionalData(EmvTag.PurposeOfTransaction, purpose);
        }

        /// <summary>
        /// Adds Additional Consumer Data Request to Additional Data Field (Tag 62, Sub-tag 09)
        /// </summary>
        /// <param name="consumerDataRequest">The additional consumer data request</param>
        public EmvBuilder SetAdditionalConsumerDataRequest(string consumerDataRequest)
        {
            return AddAdditionalData(EmvTag.AdditionalConsumerDataRequest, consumerDataRequest);
        }

        /// <summary>
        /// Removes a tag from the QR code
        /// </summary>
        /// <param name="tag">The tag to remove</param>
        public EmvBuilder RemoveData(string tag)
        {
            _qr.RemoveData(tag);
            return this;
        }

        /// <summary>
        /// Builds the EMV QR code
        /// </summary>
        /// <param name="validateBeforeGenerating">If true, validates the QR code before generating</param>
        public string Build(bool validateBeforeGenerating = false)
        {
            return _qr.GeneratePayload(validateBeforeGenerating);
        }

        /// <summary>
        /// Gets the underlying <see cref="EmvQrCode"/> instance
        /// </summary>
        public EmvQrCode GetQrCode()
        {
            return _qr;
        }
    }
}

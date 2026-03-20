# 🏦 EmvQr — EMVCo QR Code Library for .NET

[![NuGet version](https://img.shields.io/nuget/v/EmvQr.svg)](https://www.nuget.org/packages/EmvQr/)
[![NuGet downloads](https://img.shields.io/nuget/dt/EmvQr.svg)](https://www.nuget.org/packages/EmvQr/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

A lightweight, **zero-dependency** .NET 8 library for generating, parsing, and validating **EMVCo Merchant Presented QR Codes** (MPM).  
Fully compliant with the EMVCo QR Code Specification for Payment Systems and ISO/IEC 18004.

---

## ✨ Features

- **Fluent Builder API** — Construct valid EMVCo strings with method chaining
- **Parser** — Read, inspect, and modify existing EMVCo QR strings
- **Validator** — Ensure mandatory EMVCo fields are present and correctly formatted
- **CRC16-CCITT** — Automatic checksum generation per ISO/IEC 13239
- **Nested Data Support** — Handles complex structures like Additional Data Field (Tag 62) and Merchant Account Information (Tags 02-51)
- **ISO Standards Built-in** — Constants for Merchant Category Codes (ISO 18245), Country Codes (ISO 3166), and Currency Codes (ISO 4217)
- **Typed Exceptions** — 6 specific exception types for granular error handling
- **No external dependencies** — Pure .NET 8, ready for production

---

## 📦 Installation

### Via NuGet

```bash
dotnet add package EmvQr
```

### From Source

```bash
git clone https://github.com/khalilbenaz/QRLIB.git
cd QRLIB
dotnet restore
dotnet build
```

---

## 🚀 Quick Start

### Generate a QR Code

```csharp
using EmvQr;
using EmvQr.Standards;

var qrString = new EmvBuilder()
    .SetPayloadFormatIndicator("01")
    .SetPointOfInitiationMethod(false)             // Static QR (11)
    .AddMerchantAccountInformation("26", "MERCHANT_ID_123")
    .SetMerchantCategoryCode(MerchantCategoryCodes.GroceryStoresSupermarkets)
    .SetTransactionCurrency(Currencies.EUR)        // 978
    .SetCountryCode(Countries.FR)                  // FR
    .SetTransactionAmount(24.99)
    .SetMerchantName("Super Market")
    .SetMerchantCity("Paris")
    .Build();

Console.WriteLine(qrString);
// Output: 000201010211261600MERCHANT_ID_12352045411530397854052...6304XXXX
```

### Parse an Existing QR Code

```csharp
using EmvQr;

string rawQr = "000201010211...";
var qr = EmvParser.Parse(rawQr);

string merchantName = qr.Get(EmvTag.MerchantName)?.Value;
string amount       = qr.Get(EmvTag.TransactionAmount)?.Value;
string country      = qr.Get(EmvTag.CountryCode)?.Value;

// Access nested data (Tag 62 — Additional Data)
var additionalData = qr.Get(EmvTag.AdditionalDataFieldTemplate);
if (additionalData is { IsNested: true })
{
    var billNumber = additionalData.NestedData
        .FirstOrDefault(x => x.Tag == EmvTag.BillNumber)?.Value;
}
```

### Validate a QR Code

```csharp
using EmvQr;

var qr = EmvParser.Parse(rawQr);
var result = EmvValidator.Validate(qr);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
        Console.WriteLine($"❌ {error}");
}

foreach (var warning in result.Warnings)
    Console.WriteLine($"⚠️ {warning}");
```

---

## 📖 API Reference

### `EmvBuilder` — Fluent QR Code Construction

| Method | Tag | Description |
|--------|-----|-------------|
| `SetPayloadFormatIndicator(string)` | 00 | EMVCo version (default: `"01"`) |
| `SetPointOfInitiationMethod(bool)` | 01 | `false` = Static (11), `true` = Dynamic (12) |
| `AddMerchantAccountInformation(id, value)` | 02-51 | Simple merchant account info |
| `AddMerchantAccountInformation(id, guid, paymentNetworkSpecific)` | 02-51 | Nested merchant info with GUID |
| `SetMerchantCategoryCode(string)` | 52 | 4-digit MCC (use `MerchantCategoryCodes.*`) |
| `SetTransactionCurrency(string)` | 53 | 3-digit ISO 4217 code (use `Currencies.*`) |
| `SetTransactionAmount(double\|string)` | 54 | Transaction amount |
| `SetTipOrConvenienceIndicator(string)` | 55 | `01`=Tip, `02`=Fixed fee, `03`=Percentage |
| `SetValueOfConvenienceFeeFixed(double)` | 56 | Fixed convenience fee amount |
| `SetValueOfConvenienceFeePercentage(double)` | 57 | Convenience fee as percentage |
| `SetCountryCode(string)` | 58 | 2-letter ISO 3166 code (use `Countries.*`) |
| `SetMerchantName(string)` | 59 | Merchant name (max 25 chars) |
| `SetMerchantCity(string)` | 60 | Merchant city (max 15 chars) |
| `SetPostalCode(string)` | 61 | Postal code (max 10 chars) |
| `SetBillNumber(string)` | 62/01 | Bill number (nested in Tag 62) |
| `SetMobileNumber(string)` | 62/02 | Mobile number |
| `SetStoreLabel(string)` | 62/03 | Store label |
| `SetLoyaltyNumber(string)` | 62/04 | Loyalty number |
| `SetReferenceLabel(string)` | 62/05 | Reference label |
| `SetCustomerLabel(string)` | 62/06 | Customer label |
| `SetTerminalLabel(string)` | 62/07 | Terminal label |
| `SetPurposeOfTransaction(string)` | 62/08 | Purpose of transaction |
| `RemoveData(string tag)` | — | Remove a tag from the QR code |
| `Build(bool validate = false)` | — | Generate the final EMVCo string with CRC |
| `GetQrCode()` | — | Get the underlying `EmvQrCode` object |

### `EmvParser` — Parse Existing QR Strings

```csharp
// Basic parsing
EmvQrCode qr = EmvParser.Parse(rawQrString);

// Parse with automatic validation
EmvQrCode qr = EmvParser.Parse(rawQrString, validateAfterParsing: true);
```

### `EmvValidator` — Validate EMVCo Compliance

```csharp
// Get validation result
EmvValidationResult result = EmvValidator.Validate(qrCode);
bool isValid = result.IsValid;
List<string> errors = result.Errors;
List<string> warnings = result.Warnings;

// Or throw on invalid
EmvValidator.ValidateAndThrow(qrCode);  // throws EmvValidationException
```

**Validated fields:** Payload Format Indicator (00), Point of Initiation Method (01), Merchant Account Info (02-51, at least one required), MCC (52), Currency (53), Country (58), Merchant Name (59), Merchant City (60), CRC format (63), and all Additional Data fields (62).

### `EmvQrCode` — Core Data Model

```csharp
qr.AddData(tag, value);                    // Add simple TLV
qr.AddNestedData(tag, nestedList);          // Add nested TLV
qr.UpdateData(tag, newValue);               // Update existing tag
qr.RemoveData(tag);                         // Remove tag
qr.Get(tag);                                // Get EmvDataObject or null
qr.Contains(tag);                           // Check tag existence
qr.GeneratePayload(validate: false);        // Build final string with CRC
```

### `Crc16` — Checksum

```csharp
string crc = Crc16.Compute("0002010102116304");  // Returns e.g. "AD0A"
```

Manual CRC validation:

```csharp
string raw = "000201...6304ABCD";
string dataWithoutCrc = raw[..^4];
string providedCrc = raw[^4..];
string calculated = Crc16.Compute(dataWithoutCrc);

bool isValid = (providedCrc == calculated);
```

---

## 🛡️ Exception Handling

The library provides typed exceptions for precise error management:

| Exception | When Thrown |
|-----------|------------|
| `EmvQrException` | Base class for all EMV QR errors |
| `EmvParserException` | Malformed QR string, invalid length, data overflow |
| `EmvValidationException` | Missing mandatory tags, format errors (contains `.ValidationResult`) |
| `EmvBuilderException` | Builder construction errors |
| `InvalidTagException` | Invalid tag identifier (non-numeric, out of range) |
| `InvalidTagValueException` | Invalid value for a given tag |

```csharp
try
{
    var qr = EmvParser.Parse(rawQr, validateAfterParsing: true);
}
catch (EmvParserException ex)    { /* Malformed string */ }
catch (EmvValidationException ex){ /* ex.ValidationResult.Errors */ }
catch (EmvQrException ex)        { /* Catch-all for EMV errors */ }
```

---

## 🌍 ISO Standards Constants

### Currencies (`EmvQr.Standards.Currencies`)

```csharp
Currencies.MAD  // "504" — Moroccan Dirham
Currencies.EUR  // "978" — Euro
Currencies.USD  // "840" — US Dollar
Currencies.IsValid("978")  // true
```

### Countries (`EmvQr.Standards.Countries`)

```csharp
Countries.MA  // "MA" — Morocco
Countries.FR  // "FR" — France
Countries.US  // "US" — United States
Countries.IsValid("FR")  // true
```

### Merchant Category Codes (`EmvQr.Standards.MerchantCategoryCodes`)

```csharp
MerchantCategoryCodes.GroceryStoresSupermarkets  // "5411"
MerchantCategoryCodes.Restaurants                 // ...
```

---

## 🏗️ Architecture

```
QRLIB/
├── EmvQr.sln                    # Solution file
├── EmvQr/                       # Main library
│   ├── EmvBuilder.cs            # Fluent API for constructing QR strings
│   ├── EmvParser.cs             # Parse existing QR strings
│   ├── EmvValidator.cs          # Validate EMVCo compliance
│   ├── EmvQrCode.cs             # Core QR code representation
│   ├── EmvDataObject.cs         # Tag-Length-Value data objects
│   ├── EmvTag.cs                # EMVCo tag constants
│   ├── Crc16.cs                 # CRC16-CCITT checksum (ISO 13239)
│   ├── EmvExceptions.cs         # 6 typed exceptions
│   ├── EmvQr.csproj             # .NET 8 project (v1.2.4)
│   └── Standards/
│       ├── Iso18245.cs          # Merchant Category Codes
│       ├── Iso3166.cs           # Country Codes (2-letter alpha)
│       └── Iso4217.cs           # Currency Codes (3-digit numeric)
├── EmvQr.Tests/                 # Unit tests (xUnit)
│   ├── CoreTests.cs             # CRC16, Builder, Parser tests
│   ├── FeatureTests.cs          # Nested data, templates
│   └── StandardsTests.cs       # ISO code validations
└── LICENSE                      # MIT License
```

---

## 🧪 Running Tests

```bash
cd QRLIB
dotnet test
```

Tests cover CRC16 computation, builder output integrity, parser correctness, nested TLV handling, and ISO code validation.

---

## 📋 ISO Standards Compliance

| Standard | Usage in EmvQr |
|----------|----------------|
| EMVCo MPM Spec | Overall QR Code structure & tag ordering |
| ISO/IEC 13239 | CRC16-CCITT checksum calculation |
| ISO 18245 | Merchant Category Codes (Tag 52) |
| ISO 3166 | Country Codes (Tag 58) |
| ISO 4217 | Currency Codes — 504=MAD, 978=EUR, 840=USD (Tag 53) |

---

## 🤝 Contributing

```bash
git clone https://github.com/khalilbenaz/QRLIB.git
cd QRLIB
dotnet restore
dotnet test

# Create your feature branch
git checkout -b feature/my-feature
git commit -am "Add my feature"
git push origin feature/my-feature
```

Then open a Pull Request on GitHub.

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 💡 Real-World Example: Moroccan Merchant QR

```csharp
using EmvQr;
using EmvQr.Standards;

var qr = new EmvBuilder()
    .SetPointOfInitiationMethod(true)               // Dynamic
    .AddMerchantAccountInformation("26", "com.example.merchant", "ACC-789")
    .SetMerchantCategoryCode("5812")                 // Restaurants
    .SetTransactionCurrency(Currencies.MAD)          // 504 — Dirham
    .SetCountryCode(Countries.MA)                    // MA — Morocco
    .SetTransactionAmount(150.00)
    .SetMerchantName("Cafe Atlas")
    .SetMerchantCity("Casablanca")
    .SetBillNumber("INV-2026-001")
    .SetReferenceLabel("TABLE-12")
    .Build(validateBeforeGenerating: true);

// Use with any QR image library (QRCoder, ZXing.Net, etc.)
```

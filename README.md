# EmvQr - EMVCo QR Code Library for .NET

[![NuGet version](https://img.shields.io/nuget/v/EmvQr.svg)](https://www.nuget.org/packages/EmvQr/)
[![NuGet downloads](https://img.shields.io/nuget/dt/EmvQr.svg)](https://www.nuget.org/packages/EmvQr/)

A lightweight, dependency-free .NET library for generating and parsing **EMVCo Merchant Presented QR Codes** (MPM). 
Compliant with EMVCo QR Code Specification for Payment Systems (ISO/IEC 18004).

## Features

*   **Builder API**: Fluent interface for constructing valid EMVCo strings.
*   **Parser**: Read and modify existing EMVCo strings.
*   **CRC16 Calculation**: Automatic checksum generation (ISO/IEC 13239).
*   **Nested Data Support**: Handles nested objects (e.g., Additional Data Field 62).
*   **Standard Tags**: Built-in constants for common EMV tags.

## Installation

```bash
dotnet add package EmvQr
```

## Usage

### 1. Generating a QR Code

```csharp
using EmvQr;
using EmvQr.Standards; // Import definitions (New in v1.2)

var builder = new EmvBuilder()
    .SetPayloadFormatIndicator("01")
    .SetPointOfInitiationMethod(false) // Static
    
    // Use standard definitions for autocomplete
    .SetMerchantCategoryCode(MerchantCategoryCodes.GroceryStoresSupermarkets)
    .SetTransactionCurrency(Currencies.EUR)
    .SetCountryCode(Countries.FR)
    
    .SetTransactionAmount(24.99)
    .SetMerchantName("Super Market")
    .SetMerchantCity("Paris")
    .AddMerchantAccountInformation("26", "MERCHANT_ID_123");
    
// ...
```

### 2. Validation (New in v1.1)

Ensure your QR Code meets the mandatory EMVCo requirements.

```csharp
var qr = new EmvBuilder().Build(); // Missing fields
var parsedQr = EmvParser.Parse(qr);

var validation = EmvValidator.Validate(parsedQr);

if (!validation.IsValid)
{
    foreach (var error in validation.Errors)
    {
        Console.WriteLine($"Error: {error}");
    }
}
```

### 3. Parsing a QR Code

```csharp
using EmvQr;

string rawQr = "000201010211..."; // Your EMVCo string

try 
{
    var qr = EmvParser.Parse(rawQr);

    // Access simple data
    string merchantName = qr.Get(EmvTag.MerchantName)?.Value;
    string amount = qr.Get(EmvTag.TransactionAmount)?.Value;

    // Access nested data (e.g., Tag 62)
    var additionalData = qr.Get(EmvTag.AdditionalDataFieldTemplate);
    if (additionalData != null && additionalData.IsNested)
    {
        var billNumber = additionalData.NestedData
            .FirstOrDefault(x => x.Tag == EmvTag.BillNumber)?.Value;
    }
}
catch (Exception ex)
{
    Console.WriteLine("Invalid QR Code: " + ex.Message);
}
```

### 3. Validating CRC

The `EmvParser` reads the string as-is. To validate the checksum manually:

```csharp
string rawQr = "000201...6304ABCD";
string dataWithoutCrc = rawQr.Substring(0, rawQr.Length - 4);
string providedCrc = rawQr.Substring(rawQr.Length - 4);

string calculatedCrc = Crc16.Compute(dataWithoutCrc);

if (providedCrc == calculatedCrc) {
    Console.WriteLine("Valid Checksum");
}
```

## Exception Handling (New in v1.2)

The library provides specific exception types for better error handling:

```csharp
using EmvQr;

try
{
    var qr = EmvParser.Parse(rawQr);
}
catch (EmvParserException ex)
{
    // Handle parsing errors
    Console.WriteLine($"Parse error: {ex.Message}");
}
catch (EmvValidationException ex)
{
    // Handle validation errors
    foreach (var error in ex.ValidationResult.Errors)
    {
        Console.WriteLine($"Validation error: {error}");
    }
}
catch (EmvQrException ex)
{
    // Handle other EMV QR specific errors
    Console.WriteLine($"EMV QR error: {ex.Message}");
}
```

### Exception Types

| Exception | Description |
|-----------|-------------|
| `EmvQrException` | Base exception for all EMV QR related errors |
| `EmvParserException` | Thrown when parsing fails |
| `EmvValidationException` | Thrown when validation fails |
| `EmvBuilderException` | Thrown when building a QR code fails |
| `InvalidTagException` | Thrown when an invalid tag is used |
| `InvalidTagValueException` | Thrown when a tag value is invalid |

## License

MIT

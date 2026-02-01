namespace EmvQr
{
    /// <summary>
    /// Parser for EMV QR code strings
    /// </summary>
    public static class EmvParser
    {
        /// <summary>
        /// Parses a raw EMV QR code string into an <see cref="EmvQrCode"/> object
        /// </summary>
        /// <param name="rawQr">The raw EMV QR code string</param>
        /// <param name="validateAfterParsing">If true, validates the parsed QR code</param>
        public static EmvQrCode Parse(string rawQr, bool validateAfterParsing = false)
        {
            if (string.IsNullOrEmpty(rawQr))
                throw new EmvParserException("QR string cannot be empty");

            var qr = new EmvQrCode();
            int index = 0;

            while (index < rawQr.Length)
            {
                if (index + 4 > rawQr.Length) break; // Safety check

                string id = rawQr.Substring(index, 2);
                index += 2;

                string lengthStr = rawQr.Substring(index, 2);
                index += 2;

                if (!int.TryParse(lengthStr, out int length))
                    throw new EmvParserException($"Invalid length at index {index - 2}");

                if (index + length > rawQr.Length)
                    throw new EmvParserException($"Length {length} exceeds data bounds at index {index}");

                string value = rawQr.Substring(index, length);
                index += length;

                // Check if this tag usually contains nested data
                // Additional Data Field Template (62) can be nested and is treated as such by default
                if (id == EmvTag.AdditionalDataFieldTemplate)
                {
                    // Attempt to parse nested
                    try
                    {
                        var nestedObjects = ParseNested(value);
                        qr.AddNestedData(id, nestedObjects);
                    }
                    catch
                    {
                        // Fallback: treat as simple string if parsing fails
                        qr.AddData(id, value);
                    }
                }
                else
                {
                    // Merchant Account Information (02-51) and other tags are treated as simple strings
                    // They may contain TLV data but are parsed as values
                    qr.AddData(id, value);
                }
            }

            if (validateAfterParsing)
            {
                EmvValidator.ValidateAndThrow(qr);
            }

            return qr;
        }

        private static List<EmvDataObject> ParseNested(string content)
        {
            var list = new List<EmvDataObject>();
            int index = 0;
            while (index < content.Length)
            {
                if (index + 4 > content.Length) break;

                string id = content.Substring(index, 2);
                index += 2;

                string lengthStr = content.Substring(index, 2);
                index += 2;

                if (!int.TryParse(lengthStr, out int length))
                    throw new EmvParserException();

                if (index + length > content.Length)
                    throw new EmvParserException();

                string value = content.Substring(index, length);
                index += length;

                list.Add(new EmvDataObject(id, value));
            }
            return list;
        }
    }
}

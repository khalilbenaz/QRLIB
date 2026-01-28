namespace EmvQr
{
    public static class EmvParser
    {
        public static EmvQrCode Parse(string rawQr)
        {
            if (string.IsNullOrEmpty(rawQr))
                throw new ArgumentException("QR string cannot be empty");

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
                    throw new FormatException($"Invalid length at index {index - 2}");

                if (index + length > rawQr.Length)
                    throw new FormatException($"Length {length} exceeds data bounds at index {index}");

                string value = rawQr.Substring(index, length);
                index += length;

                // Check if this tag usually contains nested data
                // Merchant Account Information (02-51) and Additional Data (62) can be nested.
                // However, simple parsing usually treats everything as value unless specified.
                // For this library, we will check if it is ID 62 (Additional Data Field Template)
                // or ID 26-51 (Merchant Account Info) which *can* be nested but often aren't for basic use.
                // Let's implement a recursive parse for ID 62 as a feature.
                
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
                    qr.AddData(id, value);
                }
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
                    throw new FormatException();

                string value = content.Substring(index, length);
                index += length;

                list.Add(new EmvDataObject(id, value));
            }
            return list;
        }
    }
}

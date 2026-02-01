using System.Text;

namespace EmvQr
{
    /// <summary>
    /// Represents an EMV QR Code with all its data objects
    /// </summary>
    public class EmvQrCode
    {
        /// <summary>
        /// Gets the list of data objects in the QR code
        /// </summary>
        public List<EmvDataObject> DataObjects { get; private set; } = new List<EmvDataObject>();

        /// <summary>
        /// Adds a new data object to the QR code
        /// </summary>
        /// <param name="tag">The tag identifier</param>
        /// <param name="value">The value of the data object</param>
        /// <param name="replaceIfExists">If true, replaces existing tag with the same name</param>
        public void AddData(string tag, string value, bool replaceIfExists = true)
        {
            ValidateTag(tag);

            if (replaceIfExists)
            {
                RemoveData(tag);
            }
            else if (Get(tag) != null)
            {
                throw new InvalidTagException(tag, "Tag already exists");
            }

            DataObjects.Add(new EmvDataObject(tag, value));
        }

        /// <summary>
        /// Adds a new nested data object to the QR code
        /// </summary>
        /// <param name="tag">The tag identifier</param>
        /// <param name="nested">The nested data objects</param>
        /// <param name="replaceIfExists">If true, replaces existing tag with the same name</param>
        public void AddNestedData(string tag, List<EmvDataObject> nested, bool replaceIfExists = true)
        {
            ValidateTag(tag);

            if (replaceIfExists)
            {
                RemoveData(tag);
            }
            else if (Get(tag) != null)
            {
                throw new InvalidTagException(tag, "Tag already exists");
            }

            DataObjects.Add(new EmvDataObject(tag, nested));
        }

        /// <summary>
        /// Removes a data object from the QR code
        /// </summary>
        /// <param name="tag">The tag identifier to remove</param>
        /// <returns>True if the tag was removed, false if it wasn't found</returns>
        public bool RemoveData(string tag)
        {
            var existing = Get(tag);
            if (existing != null)
            {
                DataObjects.Remove(existing);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the value of an existing data object
        /// </summary>
        /// <param name="tag">The tag identifier to update</param>
        /// <param name="value">The new value</param>
        /// <returns>True if the tag was updated, false if it wasn't found</returns>
        public bool UpdateData(string tag, string value)
        {
            ValidateTag(tag);

            var existing = Get(tag);
            if (existing != null)
            {
                if (existing.IsNested)
                {
                    throw new InvalidTagValueException(tag, value, "Cannot update value of nested data object");
                }
                existing.Value = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the nested data of an existing data object
        /// </summary>
        /// <param name="tag">The tag identifier to update</param>
        /// <param name="nested">The new nested data objects</param>
        /// <returns>True if the tag was updated, false if it wasn't found</returns>
        public bool UpdateNestedData(string tag, List<EmvDataObject> nested)
        {
            ValidateTag(tag);

            var existing = Get(tag);
            if (existing != null)
            {
                if (!existing.IsNested)
                {
                    // Convert simple data object to nested
                    existing.IsNested = true;
                    existing.NestedData = nested;
                    existing.Value = existing.ToString();
                }
                else
                {
                    existing.NestedData = nested;
                    existing.Value = existing.ToString();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a data object by tag
        /// </summary>
        /// <param name="tag">The tag identifier to get</param>
        /// <returns>The data object or null if not found</returns>
        public EmvDataObject? Get(string tag)
        {
            return DataObjects.FirstOrDefault(d => d.Tag == tag);
        }

        /// <summary>
        /// Checks if the QR code contains a specific tag
        /// </summary>
        /// <param name="tag">The tag identifier to check</param>
        /// <returns>True if the tag exists, false otherwise</returns>
        public bool Contains(string tag)
        {
            return Get(tag) != null;
        }

        /// <summary>
        /// Generates the EMV QR code payload string
        /// </summary>
        /// <param name="validateBeforeGenerating">If true, validates the QR code before generating the payload</param>
        /// <returns>The generated EMV QR code string with CRC</returns>
        public string GeneratePayload(bool validateBeforeGenerating = false)
        {
            if (validateBeforeGenerating)
            {
                EmvValidator.ValidateAndThrow(this);
            }

            // Build the string without CRC
            StringBuilder sb = new StringBuilder();

            // Sort tags according to EMVCo specification
            // Payload Format Indicator (00) must come first
            // Then Point of Initiation Method (01)
            // Then other tags in numerical order
            var sortedObjects = DataObjects.Where(x => x.Tag != EmvTag.CRC)
                .OrderBy(obj =>
                {
                    if (obj.Tag == EmvTag.PayloadFormatIndicator) return 0;
                    if (obj.Tag == EmvTag.PointOfInitiationMethod) return 1;
                    if (int.TryParse(obj.Tag, out int tagNum)) return tagNum + 2;
                    return int.MaxValue;
                });

            foreach (var obj in sortedObjects)
            {
                sb.Append(obj.ToString());
            }

            // Append CRC ID and Length
            sb.Append(EmvTag.CRC);
            sb.Append("04");

            // Calculate CRC
            string crc = Crc16.Compute(sb.ToString());
            sb.Append(crc);

            return sb.ToString();
        }

        private void ValidateTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new InvalidTagException(tag, "Tag cannot be null or empty");

            if (tag.Length < 1 || tag.Length > 2)
                throw new InvalidTagException(tag, "Tag must be 1 or 2 characters long");

            if (!int.TryParse(tag, out int _))
                throw new InvalidTagException(tag, "Tag must be numeric");
        }
    }
}

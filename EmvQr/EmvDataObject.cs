using System.Text;

namespace EmvQr
{
    /// <summary>
    /// Represents a single EMV TLV (Tag-Length-Value) data object
    /// </summary>
    public class EmvDataObject
    {
        /// <summary>
        /// Gets or sets the tag identifier of this data object
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the value of this data object
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this data object contains nested data objects
        /// </summary>
        public bool IsNested { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of nested data objects if IsNested is true
        /// </summary>
        public List<EmvDataObject> NestedData { get; set; } = new List<EmvDataObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmvDataObject"/> class with a simple value
        /// </summary>
        /// <param name="tag">The tag identifier</param>
        /// <param name="value">The value of the data object</param>
        public EmvDataObject(string tag, string value)
        {
            Tag = tag;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmvDataObject"/> class with nested data
        /// </summary>
        /// <param name="tag">The tag identifier</param>
        /// <param name="nestedData">The list of nested data objects</param>
        public EmvDataObject(string tag, List<EmvDataObject> nestedData)
        {
            Tag = tag;
            IsNested = true;
            NestedData = nestedData;
            Value = ToStringInternal(nestedData);
        }

        /// <summary>
        /// Converts this data object to its TLV (Tag-Length-Value) string representation
        /// </summary>
        /// <returns>The TLV formatted string</returns>
        public override string ToString()
        {
            if (IsNested)
            {
                // Re-calculate value from nested data to ensure consistency
                Value = ToStringInternal(NestedData);
            }

            string length = Value.Length.ToString("D2");
            return $"{Tag}{length}{Value}";
        }

        /// <summary>
        /// Gets the length of the value in bytes
        /// </summary>
        public int Length => Value?.Length ?? 0;

        /// <summary>
        /// Gets a nested data object by tag
        /// </summary>
        /// <param name="tag">The tag identifier to search for</param>
        /// <returns>The nested data object or null if not found</returns>
        public EmvDataObject? GetNested(string tag)
        {
            if (!IsNested)
                return null;

            return NestedData.FirstOrDefault(d => d.Tag == tag);
        }

        private static string ToStringInternal(List<EmvDataObject> data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }
    }
}

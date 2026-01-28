using System.Text;

namespace EmvQr
{
    public class EmvDataObject
    {
        public string Tag { get; set; }
        public string Value { get; set; }
        public bool IsNested { get; set; } = false;
        
        // Nested objects if IsNested is true
        public List<EmvDataObject> NestedData { get; set; } = new List<EmvDataObject>();

        public EmvDataObject(string tag, string value)
        {
            Tag = tag;
            Value = value;
        }

        public EmvDataObject(string tag, List<EmvDataObject> nestedData)
        {
            Tag = tag;
            IsNested = true;
            NestedData = nestedData;
            Value = ToStringInternal(nestedData);
        }

        // Calculates the formatted string for this object (TLV)
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

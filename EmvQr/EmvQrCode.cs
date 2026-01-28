using System.Text;

namespace EmvQr
{
    public class EmvQrCode
    {
        public List<EmvDataObject> DataObjects { get; private set; } = new List<EmvDataObject>();

        public void AddData(string tag, string value)
        {
            // Remove existing if any (simple implementation usually overrides or appends, 
            // but for QR uniqueness usually we check. Here we append).
            DataObjects.Add(new EmvDataObject(tag, value));
        }
        
        public void AddNestedData(string tag, List<EmvDataObject> nested)
        {
            DataObjects.Add(new EmvDataObject(tag, nested));
        }

        public EmvDataObject? Get(string tag)
        {
            return DataObjects.FirstOrDefault(d => d.Tag == tag);
        }

        public string GeneratePayload()
        {
            // Build the string without CRC
            StringBuilder sb = new StringBuilder();
            
            // Ensure ID "00" (Payload Format Indicator) is first if present, though usually handled by builder
            // We just iterate order of insertion for flexibility
            foreach (var obj in DataObjects.Where(x => x.Tag != EmvTag.CRC))
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
    }
}

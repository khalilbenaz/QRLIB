using System.Text;

namespace EmvQr
{
    public static class Crc16
    {
        // CRC-16-CCITT (Polynomial 0x1021, Initial Value 0xFFFF)
        public static string Compute(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            int crc = 0xFFFF;
            int polynomial = 0x1021;

            foreach (byte b in bytes)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool bit = ((b >> (7 - i) & 1) == 1);
                    bool c15 = ((crc >> 15 & 1) == 1);
                    crc <<= 1;
                    if (c15 ^ bit) crc ^= polynomial;
                }
            }

            return (crc & 0xFFFF).ToString("X4");
        }
    }
}

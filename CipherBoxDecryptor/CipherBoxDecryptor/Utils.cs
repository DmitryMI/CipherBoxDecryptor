using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor
{
    static class Utils
    {
        public const int MaxPrintLength = 128;
        public static string PrintByteArray(byte[] bytes, int count = Int32.MaxValue)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < bytes.Length && i < count; i++)
            {
                builder.Append(bytes[i].ToString("00")).Append(" ");
            }

            string result = builder.ToString();
            if (result.Length > MaxPrintLength)
                result = result.Substring(0, MaxPrintLength);

            return result;
        }
    }
}

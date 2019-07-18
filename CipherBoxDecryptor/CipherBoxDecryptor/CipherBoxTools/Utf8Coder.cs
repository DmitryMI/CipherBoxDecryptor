using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public class Utf8Coder : IEncodingService, IDecodingService
    {
        public byte[] Decode(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public string Encode(byte[] strBytes)
        {
            return Encoding.UTF8.GetString(strBytes);
        }
    }
}

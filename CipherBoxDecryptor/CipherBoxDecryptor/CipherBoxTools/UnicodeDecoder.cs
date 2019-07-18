using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public class UnicodeCoder : IDecodingService, IEncodingService
    {
        public byte[] Decode(string str)
        {
            return Encoding.Unicode.GetBytes(str);
        }

        public string Encode(byte[] strBytes)
        {
            return Encoding.Unicode.GetString(strBytes);
        }
    }
}

using CipherBoxDecryptor.CipherBoxTools.EncodingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public class Base64Coder : IEncodingService, IDecodingService
    {
        public byte[] Decode(string str)
        {          
            return Base64Url.Decode(str);
        }

        public string Encode(byte[] strBytes)
        {
            return Base64Url.Encode(strBytes);
        }
    }
}

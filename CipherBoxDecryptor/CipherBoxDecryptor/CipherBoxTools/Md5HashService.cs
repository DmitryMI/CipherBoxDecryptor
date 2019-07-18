using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public class Md5HashService : IHashService
    {
        private MD5 _MD5;

        public Md5HashService()
        {
            _MD5 = new MD5CryptoServiceProvider();
        }

        public byte[] GetHash(byte[] data)
        {
            return _MD5.ComputeHash(data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public interface ICryptoService
    {
        byte[] Decrypt(byte[] data, byte[] password, byte[] iv);
        byte[] Encrypt(byte[] data, byte[] password, byte[] iv);
    }
}

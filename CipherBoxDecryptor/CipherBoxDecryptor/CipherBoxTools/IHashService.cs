using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public interface IHashService
    {
        byte[] GetHash(byte[] data);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    interface IBlockCryptoService : ICryptoService
    {
        int ProcessBlock(byte[] block, byte[] result);
        byte[] ProcessFinalBlock(byte[] block, int length);

        void InitBlockTransform(byte[] password, byte[] iv, CipherDirection direction);

        void SetBufferSize(int size);
        bool BlockFinalized { get; }

        int BlockSize { get; }

        bool SupportsMultiblock { get; }
    }
}

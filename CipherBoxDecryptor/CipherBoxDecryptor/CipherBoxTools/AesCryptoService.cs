using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public class AesCryptoService : IBlockCryptoService
    {
        public const CipherMode Mode = CipherMode.CBC;
        public const PaddingMode Padding = PaddingMode.PKCS7;

        private ICryptoTransform _blockTransform;
        private byte[] _transformBuffer;        

        public bool BlockFinalized => _blockTransform == null;

        public int BlockSize
        {
            get
            {
                if (_blockTransform != null)
                    return _blockTransform.InputBlockSize;
                else
                {
                    throw new BlockCipherNotInitiatedException();
                }
            }
        }

        public bool SupportsMultiblock
        {
            get
            {
                if (_blockTransform != null)
                    return _blockTransform.CanTransformMultipleBlocks;
                else
                {
                    throw new BlockCipherNotInitiatedException();
                }
            }
        }

        public byte[] Decrypt(byte[] data, byte[] password, byte[] iv)
        {
            AesCryptoServiceProvider aesCrypto = new AesCryptoServiceProvider();
            aesCrypto.Mode = Mode;
            aesCrypto.Padding = Padding;
            ICryptoTransform transform = aesCrypto.CreateDecryptor(password, iv);

            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
            return result;
        }

        public byte[] Encrypt(byte[] data, byte[] password, byte[] iv)
        {
            AesCryptoServiceProvider aesCrypto = new AesCryptoServiceProvider();
            aesCrypto.Mode = Mode;
            aesCrypto.Padding = Padding;
            ICryptoTransform transform = aesCrypto.CreateEncryptor(password, iv);

            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
            return result;
        }


        public int ProcessBlock(byte[] block, byte[] result)
        {
            if (_blockTransform == null)
                throw new BlockCipherNotInitiatedException();        
            
            if(_transformBuffer == null || block.Length > _transformBuffer.Length)
            {
                _transformBuffer = new byte[block.Length];
            }

            int bytes = _blockTransform.TransformBlock(block, 0, block.Length, _transformBuffer, 0);

            Array.Copy(_transformBuffer, result, _transformBuffer.Length);
            
            return bytes;
        }

        public byte[] ProcessFinalBlock(byte[] block, int length)
        {
            if (_blockTransform == null)
                throw new BlockCipherNotInitiatedException();

            byte[] data = _blockTransform.TransformFinalBlock(block, 0, length);
            
            _blockTransform.Dispose();

            _blockTransform = null;   

            return data;
        }


        public void InitBlockTransform(byte[] password, byte[] iv, CipherDirection direction)
        {
            AesCryptoServiceProvider aesCrypto = new AesCryptoServiceProvider();
            aesCrypto.Mode = Mode;
            aesCrypto.Padding = Padding;
            ICryptoTransform transform;
            if(direction == CipherDirection.Encryption)
            {
                transform = aesCrypto.CreateEncryptor(password, iv);
            }
            else 
            {
                transform = aesCrypto.CreateDecryptor(password, iv);
            }            

            _blockTransform = transform;            
        }

        public void SetBufferSize(int size)
        {
            if (_transformBuffer == null || _transformBuffer.Length != size)
                _transformBuffer = new byte[size];
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public class EncryptionService
    {
        public const int MultiblockMultiplier = 65536;

        private IHashService _hashService;
        private IDecodingService _decodingService;
        private ICryptoService _cryptoService;
        private byte[] 
            _pwdHash,
            _ivHash;

        public ILogger Logger { get; set; }
        public IProgressReporter ProgressReporter { get; set; }

        public EncryptionService(IHashService hashService, IDecodingService decodingService, ICryptoService cryptoService)
        {
            _hashService = hashService;
            _decodingService = decodingService;
            _cryptoService = cryptoService;
        }

        public void SetCipherData(string pwd, string iv)
        {
            byte[] decodePwd = _decodingService.Decode(pwd);
            _pwdHash = _hashService.GetHash(decodePwd);

            if(iv != null)
            {
                byte[] decodeIv = _decodingService.Decode(iv);
                _ivHash = _hashService.GetHash(decodeIv);
            }
        }

        public byte[] Process(byte[] input, CipherDirection dir)
        {
            if (_pwdHash == null)
                throw new CipherDataNotSetException();

            byte[] result;
            if (dir == CipherDirection.Encryption)
            {
                result =  _cryptoService.Encrypt(input, _pwdHash, _ivHash);                
            }
            else
            {
               result = _cryptoService.Decrypt(input, _pwdHash, _ivHash);                
            }

            return result;
        }

        public string Process(string inputStr, CipherDirection dir, IEncodingService encoder, IDecodingService decoder)
        {
            if (_pwdHash == null)
                throw new CipherDataNotSetException();

            byte[] input = decoder.Decode(inputStr);               

            byte[] result;
            if (dir == CipherDirection.Encryption)
            {
                result = _cryptoService.Encrypt(input, _pwdHash, _ivHash);
            }
            else
            {
                result = _cryptoService.Decrypt(input, _pwdHash, _ivHash);
            }

            return encoder.Encode(result);
        }


        public void Process(Stream input, Stream output, long inputLength, CipherDirection dir)
        {
            if (_pwdHash == null)
                throw new CipherDataNotSetException();

            

            IBlockCryptoService svc = (IBlockCryptoService)_cryptoService;
            svc.InitBlockTransform(_pwdHash, _ivHash, dir);

            int blockSize = svc.BlockSize;

            if(svc.SupportsMultiblock)
            {
                blockSize *= MultiblockMultiplier;                
            }

            svc.SetBufferSize(blockSize);

            byte[] blockBuffer = new byte[blockSize];
            int totalBytes = 0;
            bool isFinalBlock = false;

            double progress = 0;
            double progressIncrement = (double)blockSize / inputLength;

            while (totalBytes < inputLength || !isFinalBlock)
            {
                int readBytes = input.Read(blockBuffer, 0, blockSize);
                
                isFinalBlock = (inputLength - totalBytes) <= blockSize;
                

                totalBytes += readBytes;
                progress += progressIncrement;

                if((int)(progress * 100) % 10 == 0)
                {
                    ProgressReporter?.ReportProgress("N/A", progress * 100);
                }                

                //Logger?.Log("Processed: {0}", (double)totalBytes / inputLength);                

                byte[] result;
                int length;

                if (isFinalBlock)
                {
                    result = svc.ProcessFinalBlock(blockBuffer, readBytes);
                    length = result.Length;
                    //Logger.Log("FinalBlock {0} ---> {1}", Utils.PrintByteArray(blockBuffer, readBytes), Utils.PrintByteArray(result));
                }
                else
                {
                    
                    result = new byte[blockSize];
                    length = svc.ProcessBlock(blockBuffer, result);
                    //Logger.Log("Block {0} ---> {1}", Utils.PrintByteArray(blockBuffer, readBytes), Utils.PrintByteArray(result));
                }

                output.Write(result, 0, length);
            }            
        }
    }
}

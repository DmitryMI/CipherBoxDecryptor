using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    class CipherBoxProcesser
    {
        public const string NamePrefix = ".cb_";

        ILogger _logger;
        IProgressReporter _reporter;
        EncryptionService _svc;

        public CipherBoxProcesser(string password, string iv)
        {
            IHashService md5 = new Md5HashService();
            IDecodingService passwordDecoder = new Utf8Coder();
            ICryptoService aes = new AesCryptoService();
            _svc = new EncryptionService(md5, passwordDecoder, aes);
            _svc.SetCipherData(password, iv);
        }

        public ILogger Logger { get => _logger; set { _logger = value; _svc.Logger = value; } }
        public IProgressReporter Reporter { get => _reporter; set { _reporter = value; _svc.ProgressReporter = value; } }

        public void ProcessFile(FileInfo fileInfo)
        {
            ProcessFileinternal(fileInfo);
        }

        public async void ProcessFileAsync(FileInfo fileInfo)
        {
            await GetProcessFileTask(fileInfo);
        }

        private Task GetProcessFileTask(FileInfo file)
        {
            Task task = new Task(ProcessFileWrapped, file);
            return task;
        }

        private void ProcessFileWrapped(object fileWrapper)
        {
            ProcessFileinternal((FileInfo)fileWrapper);
        }

        private void ProcessFileinternal(FileInfo fileInfo)
        {
            FileStream input = null;
            FileStream output = null;

            try
            {
                Base64Coder base64Coder = new Base64Coder();
                Utf8Coder utf8Coder = new Utf8Coder();

                string name = fileInfo.Name;
                string outputName;
                CipherDirection direction;
                if (name.StartsWith(NamePrefix))
                {
                    direction = CipherDirection.Decryption;
                    string dePrefixedName = name.Remove(0, NamePrefix.Length);

                    try
                    {
                        outputName = _svc.Process(dePrefixedName, direction, utf8Coder, base64Coder);
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Error in name decryption.");
                        outputName = dePrefixedName + ".DECR";
                    }

                }
                else
                {
                    direction = CipherDirection.Encryption;

                    outputName = NamePrefix + _svc.Process(name, direction, base64Coder, utf8Coder);
                }

                input = fileInfo.OpenRead();
                output = new FileStream(outputName, FileMode.CreateNew);

                _svc.Process(input, output, input.Length, direction);
                
                output.Flush();                
            }
            catch(Exception ex)
            {
                Logger.Log("Error occured: " + ex.Message);
            }
            finally
            {
                if (input != null)
                    input.Close();
                if (output != null)
                    output.Close();
            }
        }
    }
}

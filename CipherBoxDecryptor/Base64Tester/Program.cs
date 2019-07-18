using CipherBoxDecryptor.CipherBoxTools;
using CipherBoxDecryptor.CipherBoxTools.EncodingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            TestNameEncryption();

            Console.ReadKey();
        }

        static void TestNameEncryption()
        {
            string fileName = "Обучение CRM.docx";
            Console.WriteLine("Source name: " + fileName);

            IHashService md5 = new Md5HashService();
            IDecodingService unicodeDecoder = new Utf8Coder();
            ICryptoService aes = new AesCryptoService();
            EncryptionService svc = new EncryptionService(md5, unicodeDecoder, aes);
            svc.SetCipherData("qazqaz", "qwer");

            string ecryptedName = svc.Process(fileName, CipherDirection.Encryption, new Base64Coder(), new Utf8Coder());

            Console.WriteLine("Encrypted name: " + ecryptedName);
            
            string decryptedName = svc.Process(ecryptedName, CipherDirection.Decryption, new Utf8Coder(), new Base64Coder());

            Console.WriteLine("Decrypted name: " + decryptedName);
        }
    }
}

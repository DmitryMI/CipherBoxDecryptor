using CipherBoxDecryptor.CipherBoxTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor
{
    class Program
    {

        public const int Length = 1024;

        private struct ArgrumetsParseResults
        {
            public string Password;
            public string Iv;
            public IList<string> Paths;
        }

        static void Main(string[] args)
        {           
            ConsoleLogger consoleLogger = new ConsoleLogger();
            LogReporter reporter = new LogReporter(consoleLogger);
            

            ArgrumetsParseResults argParse = ParseArguments(args);
            string password = String.Empty, iv = String.Empty;

            if (argParse.Password == null)
            {
                password = RetreivePassword();                
            }

            if (argParse.Iv == null)
            {                
                iv = RetreiveIv();
            }

            IList<FileInfo> files = GetFiles(argParse.Paths);

            CipherBoxProcesser processer = new CipherBoxProcesser(password, iv);
            processer.Logger = consoleLogger;
            processer.Reporter = reporter;
            

            foreach (var file in files)
            {
                processer.ProcessFile(file);
            }

            Console.WriteLine("Finished");
            Console.ReadKey();
        }

        static ArgrumetsParseResults ParseArguments(string[] args)
        {
            ArgrumetsParseResults results = new ArgrumetsParseResults();
            results.Paths = new List<string>();
            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].Equals("/p"))
                {
                    i++;
                    results.Password = args[i];
                }
                else if (args[i].Equals("/iv"))
                {
                    i++;
                    results.Iv = args[i];
                }
                else
                {
                    results.Paths.Add(args[i]);
                }
            }

            return results;
        }
        
        static IList<FileInfo> GetFiles(IList<string> paths)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();

            foreach(var path in paths)
            {
                if (File.Exists(path))
                {
                    fileInfos.Add(new FileInfo(path));
                }
                else if (Directory.Exists(path))
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    LoadAllFiles(dir, fileInfos, 0, 3);
                }
                else
                {
                    Console.WriteLine("Specified path does not exist!");
                }
            }

            return fileInfos;
        }

        static void LoadAllFiles(DirectoryInfo dir, IList<FileInfo> fileList, int level, int maxLevel)
        {            

            IEnumerable<FileInfo> fileInfos = dir.EnumerateFiles();
            IEnumerable<DirectoryInfo> dirInfos = dir.EnumerateDirectories();

            foreach (var file in fileInfos)
                fileList.Add(file);

            if (level < maxLevel)
            {
                foreach (var subdir in dirInfos)
                {
                    LoadAllFiles(subdir, fileList, level + 1, maxLevel);
                }
            }
        }

        static string RetreivePassword()
        {
            Console.WriteLine("Password: ");
            return ReadUserCipherData();
        }

        static string RetreiveIv()
        {
            Console.WriteLine("IV: ");
            return ReadUserCipherData();
        }

        static string ReadUserCipherData()
        {
            StringBuilder builder = new StringBuilder();
            while(true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                    break;
                else if (key.Key == ConsoleKey.Backspace)
                {
                    builder.Remove(builder.Length - 1, 1);
                    Console.Write("\b \b");                    
                }
                else
                {
                    char c = key.KeyChar;
                    builder.Append(c);
                    Console.Write("*");
                }
            }

            Console.Clear();

            return builder.ToString();
        }

        static bool CompareArrays(byte[] arr1, byte[] arr2, out int error)
        {
            error = -1;

            if(arr1.Length != arr2.Length)
            {
                error = -2;
                return false;
            }

            for(int i = 0; i < arr1.Length; i++)
            {
                if(arr1[i] != arr2[i])
                {
                    error = i;
                    return false;
                }
            }

            return true;
        }

        static void GenerateSequence(byte[] result)
        {             
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(i % 255);
            }
            
        }
    }
}

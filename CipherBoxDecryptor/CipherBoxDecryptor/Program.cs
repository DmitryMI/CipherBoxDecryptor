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

        private struct ArgumetsParseResults
        {
            public string Password;
            public string Iv;
            public string OutputDirectory;
            public bool DoPause;
            public IList<string> Paths;
        }

        static int Main(string[] args)
        {
            ConsoleLogger consoleLogger = new ConsoleLogger();
            LogReporter reporter = new LogReporter(consoleLogger);


            ArgumetsParseResults argParse = ParseArguments(args);
            string password = String.Empty, iv = String.Empty;

            if (argParse.Password == null)
            {
                password = RetreivePassword();
            }
            else
            {
                password = argParse.Password;
            }

            if (argParse.Iv == null)
            {
                iv = RetreiveIv();
            }
            else
            {
                iv = argParse.Iv;
            }

            IList<FileInfo> files;
            if (argParse.Paths != null && argParse.Paths.Count > 0)
                files = GetFiles(argParse.Paths);
            else
            {
                files = GetFilesFromConsole();
            }

            DirectoryInfo outputDir;

            if (argParse.OutputDirectory != null && !String.IsNullOrEmpty(argParse.OutputDirectory))
            {
                outputDir = new DirectoryInfo(argParse.OutputDirectory);
            }
            else
            {
                outputDir = new DirectoryInfo(Environment.CurrentDirectory);
            }

            CipherBoxProcessor processer = new CipherBoxProcessor(password, iv, outputDir, consoleLogger);
            processer.Logger = consoleLogger;
            processer.Reporter = reporter;

            int errorCount = 0;
            int totalCount = 0;
            foreach (var file in files)
            {
                totalCount++;
                try
                {
                    processer.ProcessFile(file);
                }
                catch (Exception)
                {
                    errorCount++;
                }
            }

            Console.WriteLine("TOTAL: " + totalCount);
            Console.WriteLine("ERRORS: " + errorCount);

            Console.WriteLine("Finished");

            if(argParse.DoPause)
                Console.ReadKey();

            return errorCount;
        }

        static ArgumetsParseResults ParseArguments(string[] args)
        {
            ArgumetsParseResults results = new ArgumetsParseResults();
            results.Paths = new List<string>();
            results.DoPause = true;
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("/help"))
                {
                    Console.WriteLine("/help\t\tthis help");
                    Console.WriteLine("/p <password>\t\tPassword");
                    Console.WriteLine("/iv <iv>\t\tInitialization vector");
                    Console.WriteLine("/out <path>\t\tOutput folder");
                    Console.WriteLine("/nosuspend\t\tApplication will exit immidiately");
                    Console.WriteLine("<...paths...>\t\tAll arguments without /<modifier> will be considered as input files and directories.");
                }
                else if (args[i].Equals("/nosuspend"))
                {
                    results.DoPause = false;
                }
                else if(args[i].Equals("/p"))
                {
                    i++;
                    results.Password = args[i];
                }
                else if (args[i].Equals("/iv"))
                {
                    i++;
                    results.Iv = args[i];
                }
                else if (args[i].Equals("/out"))
                {
                    i++;
                    results.OutputDirectory = args[i];
                }
                else
                {
                    results.Paths.Add(args[i]);
                }
            }

            return results;
        }
        
        static IList<FileInfo> GetFiles(ICollection<string> paths)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();

            if (paths == null || paths.Count == 0)
            {
                Console.WriteLine("Nothing to process.");
            }
            else
            {
                foreach (var path in paths)
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
                        Console.WriteLine("Path {0} does not exist!", path);
                    }
                }
            }

            return fileInfos;
        }

        static IList<FileInfo> GetFilesFromConsole()
        {
            Console.WriteLine("Enter paths in quotes using space as separator: ");
            string input = Console.ReadLine();
            string[] paths = input?.Split(' ');

            return GetFiles(paths);
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

using CipherBoxDecryptor.CipherBoxTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor
{
    class ConsoleLogger : ILogger, IProgressReporter
    {
        private object _locker = new object();

        public void Log(string format, params object[] args)
        {
            string message = String.Format(format, args);
            lock (_locker)
            {
                Console.WriteLine(message);
            }
        
        }

        public void ReportFinish(object message, bool success)
        {
            throw new NotImplementedException();
        }

        public void ReportProgress(object message, double progress)
        {
            throw new NotImplementedException();
        }
    }
}

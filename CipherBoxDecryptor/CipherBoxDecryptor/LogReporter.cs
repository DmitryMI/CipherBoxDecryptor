using CipherBoxDecryptor.CipherBoxTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor
{
    class LogReporter : IProgressReporter
    {
        private ILogger _logger;

        public LogReporter(ILogger logger)
        {
            _logger = logger;
        }

        public void ReportFinish(object message, bool success)
        {
            if (success)
            {
                _logger.Log("{0} finished. Succesfully processed");
            }
            else
            {
                _logger.Log("{0} finished. An error occured");
            }
        }

        public void ReportProgress(object message, double progress)
        {
            _logger.Log("Progress of {0}: {1}", message, progress.ToString("0.00"));
        }
    }
}

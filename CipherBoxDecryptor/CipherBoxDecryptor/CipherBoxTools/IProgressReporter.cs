using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public interface IProgressReporter
    {
        void ReportProgress(object message, double progress);
        void ReportFinish(object message, bool success);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherBoxDecryptor.CipherBoxTools
{
    public interface ILogger
    {
        void Log(string format, params object[] args);
    }
}

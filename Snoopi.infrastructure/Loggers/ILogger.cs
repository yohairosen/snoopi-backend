using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.infrastructure.Loggers
{
    public interface ILogger
    {
        void LogText(string header, string text, string targetLocation);
    }
}

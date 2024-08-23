using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgSeeker.EventHandlers.Util
{
    public interface ITgsServiceLogger
    {
        void LogInfo(string message);
        void LogError(string message);
    }
}

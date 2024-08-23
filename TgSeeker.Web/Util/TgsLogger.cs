using TgSeeker.EventHandlers.Util;

namespace TgSeeker.Web.Util
{
    public class TgsLogger : ITgsServiceLogger
    {
        public void LogError(string message)
        {
            Console.WriteLine(message);
        }

        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }
    }
}

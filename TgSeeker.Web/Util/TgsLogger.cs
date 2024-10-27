using TgSeeker.Util;

namespace TgSeeker.Web.Util
{
    public class TgsLogger : ITgsServiceLogger
    {
        public void LogError(string message)
        {
            WriteMessage(FormatMessage(message, "error"));
        }

        public void LogInfo(string message)
        {
            WriteMessage(FormatMessage(message, "info"));
        }

        private string FormatMessage(string message, string tag)
        {
            return $"{DateTime.Now.ToString("T")} [{tag}] {message}";
        }

        private void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}

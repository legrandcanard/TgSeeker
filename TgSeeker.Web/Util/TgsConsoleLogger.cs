using TgSeeker.Util;

namespace TgSeeker.Web.Util
{
    public class TgsConsoleLogger : ITgsServiceLogger
    {
        public void LogError(string message)
        {
            WriteMessage(message, "info", ConsoleColor.Red);
        }

        public void LogInfo(string message)
        {
            WriteMessage(message, "info", ConsoleColor.Green);
        }

        private void WriteMessage(string message, string tag, ConsoleColor tagColor)
        {
            Console.Write(DateTime.Now.ToString("T"));
            Console.ForegroundColor = tagColor;
            Console.Write($" [{tag}] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }
    }
}

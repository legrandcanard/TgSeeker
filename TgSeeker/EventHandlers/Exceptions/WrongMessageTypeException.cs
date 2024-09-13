
namespace TgSeeker.EventHandlers.Exceptions
{
    public class WrongMessageTypeException : ArgumentException
    {
        public WrongMessageTypeException() : base($"Wrong message type provided.") { }
    }
}

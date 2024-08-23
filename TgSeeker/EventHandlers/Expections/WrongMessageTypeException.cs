using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgSeeker.EventHandlers.Expections
{
    public class WrongMessageTypeException : ArgumentException
    {
        public WrongMessageTypeException() : base($"Wrong message type provided.") { }
    }
}

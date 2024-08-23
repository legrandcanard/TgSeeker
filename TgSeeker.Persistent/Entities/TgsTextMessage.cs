using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgSeeker.Persistent.Entities
{
    public class TgsTextMessage : TgsMessage
    {
        public string Text { get; set; } = null!;
    }
}

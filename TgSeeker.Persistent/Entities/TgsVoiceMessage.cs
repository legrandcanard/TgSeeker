using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgSeeker.Persistent.Entities
{
    public class TgsVoiceMessage : TgsMessage
    {
        public byte[] Waveform { get; set; }
        public int Duration { get; set; }
        public string LocalFileId { get; set; }
    }
}

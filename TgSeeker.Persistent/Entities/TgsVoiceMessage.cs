using TgSeeker.Persistent.Entities.Interfaces;

namespace TgSeeker.Persistent.Entities
{
    public class TgsVoiceMessage : TgsMessage, IHasResource
    {
        public byte[] Waveform { get; set; }
        public int Duration { get; set; }
        public string LocalFileId { get; set; }
    }
}

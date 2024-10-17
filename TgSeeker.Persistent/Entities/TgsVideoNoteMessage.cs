
using TgSeeker.Persistent.Entities.Interfaces;

namespace TgSeeker.Persistent.Entities
{
    public class TgsVideoNoteMessage : TgsMessage, IHasResource
    {
        public int Duration { get; set; }
        public byte[] Waveform { get; set; }
        public int Length { get; set; }
        public string LocalFileId { get; set; }
    }
}

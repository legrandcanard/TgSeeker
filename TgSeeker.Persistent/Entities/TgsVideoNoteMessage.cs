
using System.Text.Json.Serialization;
using System;

namespace TgSeeker.Persistent.Entities
{
    public class TgsVideoNoteMessage : TgsMessage
    {
        public int Duration { get; set; }
        public byte[] Waveform { get; set; }
        public int Length { get; set; }
        public string LocalFileId { get; set; }

        #region Minithumbnail
        public int MinithumbnailWidth { get; set; }
        public int MinithumbnailHeight { get; set; }
        public byte[] MinithumbnailData { get; set; }
        #endregion

        #region Thumbnail
        public string ThumbnailFormat { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public string ThumbnailLocalFileId { get; set; }
        #endregion
    }
}

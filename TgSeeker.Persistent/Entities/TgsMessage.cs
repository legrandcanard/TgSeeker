
namespace TgSeeker.Persistent.Entities
{
    public partial class TgsMessage
    {
        public long Id { get; set; }
        public long ChatId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

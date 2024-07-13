
namespace TgSeeker.Persistent.Entities
{
    public partial class Message
    {
        public long Id { get; set; }
        public long ChatId { get; set; }
        public string Text { get; set; } = null!;
    }
}

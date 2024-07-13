using TgSeeker.Persistent.Entities;

namespace TgSeeker.Persistent.Repositiories
{
    public interface IMessagesRepository
    {
        Task<Message[]> GetMessagesAsync(long chatId, long[] messageIds);
        Task CreateMessage(Message message);
        Task DeleteMessage(long messageId);
    }
}

using TgSeeker.Persistent.Entities;

namespace TgSeeker.Persistent.Repositiories
{
    public interface IMessagesRepository
    {
        Task<TgsMessage[]> GetMessagesAsync(long chatId, long[] messageIds);
        Task CreateMessage(TgsMessage message);
        Task DeleteMessage(long messageId);
    }
}

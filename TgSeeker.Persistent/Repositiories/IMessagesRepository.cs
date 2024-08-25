using TgSeeker.Persistent.Entities;

namespace TgSeeker.Persistent.Repositiories
{
    public interface IMessagesRepository
    {
        Task<TgsMessage[]> GetMessagesAsync(long chatId, long[] messageIds);
        Task CreateMessageAsync(TgsMessage message);
        Task DeleteMessageAsync(long messageId);
        Task<TgsMessage[]> GetOldMessagesAsync();
    }
}

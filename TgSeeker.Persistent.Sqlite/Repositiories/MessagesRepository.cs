using Microsoft.EntityFrameworkCore;
using TgSeeker.Persistent.Contexts;
using TgSeeker.Persistent.Entities;

namespace TgSeeker.Persistent.Repositiories
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly ApplicationContext _context;

        public MessagesRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<TgsMessage[]> GetMessagesAsync(long chatId, long[] messageIds)
        {
            return await _context.Messages.Where(message => message.ChatId == chatId && messageIds.Contains(message.Id)).AsNoTracking().ToArrayAsync();
        }

        public async Task CreateMessageAsync(TgsMessage message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(long messageId)
        {
            var message = _context.Messages.FirstOrDefault(message => message.Id == messageId);
            if (message == null)
                return;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        public async Task<TgsMessage[]> GetOldMessagesAsync()
        {
            var date = DateTime.UtcNow.AddDays(-30);

            return await _context.Messages
                .Where(message => message.CreateDate <= date)
                .AsNoTracking()
                .ToArrayAsync();
        }
    }
}

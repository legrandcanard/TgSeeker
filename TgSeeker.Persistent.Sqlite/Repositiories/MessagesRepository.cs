using Microsoft.EntityFrameworkCore;
using TgSeeker.Persistent.Contexts;
using TgSeeker.Persistent.Entities;

namespace TgSeeker.Persistent.Repositiories
{
    public class MessagesRepository : IMessagesRepository
    {
        public async Task<Message[]> GetMessagesAsync(long chatId, long[] messageIds)
        {
            using var context = new ApplicationContext();
            return await context.Messages.Where(message => message.ChatId == chatId && messageIds.Contains(message.Id)).AsNoTracking().ToArrayAsync();
        }

        public async Task CreateMessage(Message message)
        {
            using var context = new ApplicationContext();
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMessage(long messageId)
        {
            using var context = new ApplicationContext();
            var message = context.Messages.FirstOrDefault(message => message.Id == messageId);
            if (message == null)
                return;

            context.Messages.Remove(message);
            await context.SaveChangesAsync();
        }
    }
}

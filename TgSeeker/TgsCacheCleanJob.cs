
using System.Diagnostics;
using TgSeeker.Persistent.Entities.Interfaces;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Util;

namespace TgSeeker
{
    public class TgsCacheCleanJob
    {
        public const string JobKey = "CacheCleanJob";

        private readonly IMessagesRepository _messagesRepository;

        public TgsCacheCleanJob(IMessagesRepository messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }

        public async Task ExecuteAsync()
        {
            //Debug.WriteLine("TgsCacheCleanJob started");
            //var messages = await _messagesRepository.GetMessagesOlderThenAsync(DateTime.UtcNow.AddMinutes(-5));
            //foreach (var message in messages)
            //{
            //    if (message is IHasResource messageWithLocalFile)
            //    {
                    
            //    }
            //}
        }
    }
}

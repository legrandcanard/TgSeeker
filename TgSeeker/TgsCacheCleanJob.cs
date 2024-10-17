
using System.Diagnostics;
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
            Debug.WriteLine("TgsCacheCleanJob started");
            var messages = await _messagesRepository.GetMessagesOlderThenAsync(DateTime.UtcNow.AddDays(-5));

            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (var message in messages)
            {
                await TgsMessageEvent.For(message, null, _messagesRepository, null).RemoveCacheForMessageAsync(message);
            }

            sw.Stop();
            Debug.WriteLine($"TgsCacheCleanJob took {sw.Elapsed.TotalMilliseconds} ms");
        }
    }
}

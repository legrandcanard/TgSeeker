
using Microsoft.EntityFrameworkCore;
using TgSeeker.Persistent.Contexts;
using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;

namespace TgSeeker.Persistent.Sqlite.Repositiories
{
    public class SettingsRepository : ISettingsRepository
    {
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            using var context = new ApplicationContext();
            return await context.Options.AsNoTracking().ToDictionaryAsync(i => i.Key, i => i.Value);
        }

        public async Task SaveSettingsAsync(Dictionary<string, string> settings)
        {
            using var context = new ApplicationContext();
            var options = settings.Select(i => new Option { Key =  i.Key, Value = i.Value });
            context.Options.RemoveRange(context.Options.AsEnumerable());
            await context.Options.AddRangeAsync(options);
            await context.SaveChangesAsync();
        }
    }
}

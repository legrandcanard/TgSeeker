
namespace TgSeeker.Persistent.Repositiories
{
    public interface ISettingsRepository
    {
        public Task<Dictionary<string, string>> GetSettingsAsync();
        public Task SaveSettingsAsync(Dictionary<string, string> settings);
    }
}

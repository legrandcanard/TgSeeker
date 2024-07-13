
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Persistent.Sqlite.Repositiories;

namespace TgSeeker.Web.Services
{
    public class TgSeekerHostedService : TgSeekerService, IHostedService
    {
        public TgSeekerHostedService() : base(new MessagesRepository(), new SettingsRepository()) 
        { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync();
        }
    }
}

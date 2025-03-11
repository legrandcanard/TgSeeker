
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Persistent.Sqlite.Repositiories;
using TgSeeker.Statistics;
using TgSeeker.Util;

namespace TgSeeker.Web.Services
{
    public class TgSeekerHostedService : TgSeekerService, IHostedService
    {
        public TgSeekerHostedService(
            IMessagesRepository messagesRepository, 
            SettingsRepository settingsRepository,
            IMetricsReporter metricsReporter,
            ITgsServiceLogger logger) : base(messagesRepository, settingsRepository, metricsReporter, logger) 
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

namespace TgSeeker.Statistics
{
    public class MetricsReporter : IMetricsReporter
    {
        private readonly MetricsStorage _metricsStorage;

        public MetricsReporter(MetricsStorage metricsStorage)
        {
            _metricsStorage = metricsStorage;
        }

        public void IncrementByOne(Metrics metric)
        {
            _metricsStorage.UpdateMetric(metric, 1);
        }
    }
}

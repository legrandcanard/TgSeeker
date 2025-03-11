using System.Collections.Concurrent;

namespace TgSeeker.Statistics
{
    public class MetricsStorage
    {
        readonly ConcurrentDictionary<Metrics, int> _metrics = [];

        public MetricsStorage()
        {
            foreach (var metric in Enum.GetValues<Metrics>())
            {
                _metrics.TryAdd(metric, default);
            }
        }

        public IReadOnlyDictionary<Metrics, int> Metrics => _metrics;

        public void UpdateMetric(Metrics metric, int value)
        {
            _metrics.AddOrUpdate(metric, value, 
                (metric, currentValue) => currentValue + value);
        }
    }
}

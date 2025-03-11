namespace TgSeeker.Statistics
{
    public interface IMetricsReporter
    {
        public void IncrementByOne(Metrics metric);
    }
}

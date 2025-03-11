using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TgSeeker.Statistics;

namespace TgSeeker.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController(MetricsStorage _metricsStorage) : ControllerBase
    {
        [HttpGet]
        public IReadOnlyDictionary<Metrics, int> Get()
        {
            return _metricsStorage.Metrics;
        }
    }
}

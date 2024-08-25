using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Web.Models;

namespace TgSeeker.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingsController(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        [HttpGet]
        public async Task<SettingsModel> Get()
        {
            var settings = await _settingsRepository.GetSettingsAsync();
            var model = new SettingsModel
            {
                ApiId = settings.ContainsKey("ApiId") ? settings["ApiId"] : string.Empty,
                ApiHash = settings.ContainsKey("ApiHash") ? settings["ApiHash"] : string.Empty
            };
            return model;
        }

        [HttpPost]
        public async Task Post([FromBody] SettingsModel value)
        {
            await _settingsRepository.SaveSettingsAsync(new Dictionary<string, string>
            {
                { "ApiId", value.ApiId },
                { "ApiHash", value.ApiHash }
            });
        }
    }
}

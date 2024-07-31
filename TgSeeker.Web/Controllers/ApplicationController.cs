using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TgSeeker.Web.Services;

namespace TgSeeker.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly TgSeekerHostedService _tgSeekerHostedService;

        public ApplicationController(TgSeekerHostedService tgSeekerHostedService)
        {
            _tgSeekerHostedService = tgSeekerHostedService;
        }

        [HttpGet("serviceState")]
        public TgSeekerHostedService.ServiceStates ServiceState()
        {
            return _tgSeekerHostedService.ServiceState;
        }

        [HttpPost("startService")]
        public async Task StartService()
        {
            if (_tgSeekerHostedService.ServiceState == TgSeekerService.ServiceStates.Idle)
                await _tgSeekerHostedService.StartAsync();
        }

        [HttpPost("stopService")]
        public async Task StopService()
        {
            if (_tgSeekerHostedService.ServiceState == TgSeekerService.ServiceStates.Running)
                await _tgSeekerHostedService.StopAsync();
        }
    }
}

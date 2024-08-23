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
        public TgSeekerService.ServiceStates ServiceState()
        {
            return _tgSeekerHostedService.ServiceState;
        }

        [HttpPost("startService")]
        public async Task<TgSeekerService.ServiceStates> StartService()
        {
            if (_tgSeekerHostedService.ServiceState != TgSeekerService.ServiceStates.Running)
                await _tgSeekerHostedService.StartAsync();

            return _tgSeekerHostedService.ServiceState;
        }

        [HttpPost("stopService")]
        public async Task<TgSeekerService.ServiceStates> StopService()
        {
            if (_tgSeekerHostedService.ServiceState == TgSeekerService.ServiceStates.Running)
                await _tgSeekerHostedService.StopAsync();

            return _tgSeekerHostedService.ServiceState;
        }
    }
}

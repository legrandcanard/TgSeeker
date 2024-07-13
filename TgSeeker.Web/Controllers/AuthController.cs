using Microsoft.AspNetCore.Mvc;
using TdLib;
using TgSeeker.Web.Models;
using TgSeeker.Web.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TgSeeker.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TgSeekerHostedService _tgSeekerHostedService;

        public AuthController(TgSeekerHostedService tgSeekerHostedService)
        {
            _tgSeekerHostedService = tgSeekerHostedService;
        }

        [HttpGet("authorizationState")]
        public TgSeekerHostedService.AuthStates AuthorizationState()
        {
            return _tgSeekerHostedService.AuthorizationState;
        }

        [HttpGet("currentUser")]
        public TdApi.User GetCurrentUser()
        {
            return _tgSeekerHostedService.CurrentUser;
        }

        [HttpPost("setPhone")]
        public async Task<ResponseModel> SetPhone([FromBody] SetPhoneRequestModel model)
        {
            try
            {
                await _tgSeekerHostedService.SendAuthenticationCodeToPhone(model.PhoneNumber);
                return new ResponseModel();
            }
            catch (TdException e)
            {
                return new ResponseModel(false, e.Message);
            }
        }

        [HttpPost("checkCode")]
        public async Task<ResponseModel> CheckCode([FromBody] SetPhoneCodeRequestModel model)
        {            
            try
            {
                await _tgSeekerHostedService.CheckAuthenticationCodeAsync(model.PhoneCode);
                return new ResponseModel();
            }
            catch (TdException e)
            {
                return new ResponseModel(false, e.Message);
            }
        }

        [HttpGet("logOut")]
        public async Task<ResponseModel> LogOut()
        {
            try
            {
                await _tgSeekerHostedService.LogOut();
                return new ResponseModel();
            }
            catch (TdException e)
            {
                return new ResponseModel(false, e.Message);
            }
        }
    }
}

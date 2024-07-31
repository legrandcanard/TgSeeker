using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TgSeeker.Web.Areas.Identity.Data;
using TgSeeker.Web.Data;
using TgSeeker.Web.Models;

using static TdLib.TdApi;
using static TgSeeker.Web.Program;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TgSeeker.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<TgsUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<TgsUser> _signInManager;

        public AccountController(
            UserManager<TgsUser> userManager,
            IConfiguration configuration,
            SignInManager<TgsUser> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("signIn")]
        public async Task<IActionResult> SignIn([FromBody] LogInModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, false);
            var test = HttpContext.User.Identity;
            if (result.Succeeded)
            {
                return Ok();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("signOut")]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TgSeeker.Web.Areas.Identity.Data;
using TgSeeker.Web.Models;
using TgSeeker.Web.Util;

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

        [HttpGet]
        [Authorize]
        [Route("details")]
        public async Task<IActionResult> GetAccountDetails()
        {
            TgsUser user = await _userManager.GetUserAsync(User) ?? throw new Exception("Failed to retrieve user");

            return new JsonResult(new TgsUserModel
            {
                UserName = user.UserName
            });
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
        [Authorize]
        [Route("signOut")]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ProblemDetails { Title = "Invalid input" });

            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest(new ProblemDetailsBuilder()
                    .ForControl(nameof(model.ConfirmNewPassword))
                    .HasMessage("Passwords should match")
                    .Create());

            TgsUser currentUser = await _userManager.GetUserAsync(base.User) ?? throw new Exception("Failed to retrieve user");

            if (!await _userManager.CheckPasswordAsync(currentUser, model.CurrentPassword))
            {
                return BadRequest(new ProblemDetailsBuilder()
                    .ForControl(nameof(model.CurrentPassword))
                    .HasMessage("Wrong password")
                    .Create());
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                IEnumerable<string> errors = result.Errors.Select(error => error.Description);

                var builder = new ProblemDetailsBuilder();
                builder.ForControl(nameof(model.NewPassword));

                foreach (var error in result.Errors)
                {
                    builder.HasMessage(error.Description);
                }

                return StatusCode(StatusCodes.Status400BadRequest, builder.Create());
            }

            return Ok();
        }
    }
}

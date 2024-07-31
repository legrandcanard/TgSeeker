using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TgSeeker.Web.Areas.Identity.Data;

namespace TgSeeker.Web.Services
{
    public class TgsUserManager : UserManager<TgsUser>
    {
        public TgsUserManager(IUserStore<TgsUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TgsUser> passwordHasher, IEnumerable<IUserValidator<TgsUser>> userValidators, IEnumerable<IPasswordValidator<TgsUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TgsUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}

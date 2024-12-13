using Microsoft.AspNetCore.Identity;

namespace meetings_app_server.Repositories
{
    public interface ITokenRepository
    {
        //string CreateJWTToken(IdentityUser user, List<string> roles);
        string CreateJWTToken(IdentityUser user);

    }
}

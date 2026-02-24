using Microsoft.AspNetCore.Identity;
using Shopping.DataAccess.Models;

namespace Online_ShoppingCart_API.Auth
{
    public interface IAuthRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}

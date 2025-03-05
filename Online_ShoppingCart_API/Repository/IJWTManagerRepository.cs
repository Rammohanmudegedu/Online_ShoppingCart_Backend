using Microsoft.AspNetCore.Identity;
using Online_ShoppingCart_API.Models;

namespace Online_ShoppingCart_API.Repository
{
    public interface IJWTManagerRepository
    {
    

        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}

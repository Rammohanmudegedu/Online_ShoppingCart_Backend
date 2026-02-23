using Microsoft.AspNetCore.Identity;
using Shopping.DataAccess.Models;

namespace Shopping.DataAccess.Repository
{
    public interface IJWTManagerRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}

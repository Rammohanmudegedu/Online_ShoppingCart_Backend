using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Shopping.DataAccess.Models;
using shopping.DataAccess.IRepositories;

namespace Online_ShoppingCart_API.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration iconfiguration;

        public AuthRepository(IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
        }

        public string CreateJWTToken(IdentityUser user, List<string> roles)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                    iconfiguration["JWT:Issuer"],
                    iconfiguration["JWT:Audience"],
                    claims, expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Online_ShoppingCart_API.Auth;
using Shopping.DataAccess.Models;

namespace Online_ShoppingCart_API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;

        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(IAuthRepository authRepository, UserManager<IdentityUser> userManager)
        {

            _authRepository = authRepository;
            _userManager = userManager;

        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Authenticate(LoginModel login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);

                if (user != null)
                {
                    var checkPassword = await _userManager.CheckPasswordAsync(user, login.Password);

                    if (checkPassword)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles != null)
                        {
                            var jwtToken = _authRepository.CreateJWTToken(user, roles.ToList());

                            var response = new Tokens
                            {
                                Token = jwtToken,
                                Roles = roles.ToList()

                            };

                            return Ok(response);

                        }
                    }
                }
                return BadRequest("Email or Password Incorrect.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreWebApiDemo.Models;
using NetCoreWebApiDemo.Services;

namespace NetCoreWebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {
            if(loginRequest.Username == "osman.erol" && loginRequest.Password == "12345")
            {
                var token = _jwtService.GenerateToken("1", loginRequest.Username);
                return Ok(new { token });
            }
            return Unauthorized("Username or password is wrong.");
        }
    }
}

using Hospital;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMediacalHistoryApi.Logic;

namespace WebMediacalHistoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserDataOperations _userDataOperations;

        public LoginController(IConfiguration configuration, UserDataOperations userDataOperations)
        {
            _configuration = configuration;
            _userDataOperations = userDataOperations;
        }

        [HttpPost("login")]
        public IActionResult Login([FromForm] string username, [FromForm] string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Invalid credentials");
            }

            var user = _userDataOperations.GetUserByUsername(username);
            if (user == null || user.Rows[0]["PasswordHash"].ToString() != password)
            {
                return Unauthorized("Invalid credentials");
            }

            TokenGeneration tokenGenerator = new TokenGeneration(_configuration);
            string token = tokenGenerator.GenerateJWT(user.Rows[0]["Username"].ToString(), user.Rows[0]["Role"].ToString());
            return Ok(new { Token = token });
        }
    }
}
using Hospital;
using Hospital.UserLogin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HospitalAPI.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cors;


namespace HospitalAPI.Controllers
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
        [EnableCors("AllowFrontend")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel userlog)
        {
            if (string.IsNullOrEmpty(userlog.Username) || string.IsNullOrEmpty(userlog.PasswordHash))
            {
                return BadRequest("Invalid credentials");
            }

            var user = await Task.Run(() => _userDataOperations.GetUserByUsername(userlog.Username));
            if (user == null || user.Rows[0]["PasswordHash"].ToString() != userlog.PasswordHash)
            {
                return Unauthorized("Invalid credentials");
            }

            //int patientID = Convert.ToInt32(user.Rows[0]["PatientID"]); 
            TokenGeneration tokenGenerator = new TokenGeneration(_configuration);
            string token = tokenGenerator.GenerateJWT(user.Rows[0]["username"].ToString(), user.Rows[0]["role"].ToString());
            return Ok(new { Token = token });
        }

    }
}
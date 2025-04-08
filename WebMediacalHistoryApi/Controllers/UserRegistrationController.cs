using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hospital;
using WebMediacalHistoryApi.Logic;
using System.Data;

namespace WebMediacalHistoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly UserDataOperations _userDataOperations;
        private readonly TokenGeneration _tokenGeneration;

        public UserRegistrationController(UserDataOperations userDataOperations, IConfiguration configuration)
        {
            _userDataOperations = userDataOperations;
            _tokenGeneration = new TokenGeneration(configuration);
        }

        [HttpPost("register")]
        public ActionResult Register(UserModel user)
        {
            int isRegistered = _userDataOperations.RegisterUser(user);
            if (isRegistered > 0)
            {
                // Generate JWT token
                string token = _tokenGeneration.GenerateJWT(user.Username, user.Role);
                return Ok(new { message = "User registered successfully", token });
            }
            return BadRequest(new { message = "User registration failed" });
        }

        [HttpGet("{username}")]
        public ActionResult<IEnumerable<UserModel>> GetUser(string username)
        {
            try
            {
                DataTable dt = _userDataOperations.GetUserByUsername(username);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return NotFound("No History found in the database.");
                }
                var userLoginList = _userDataOperations.UserConvertDataTableToList(dt);
                return Ok(userLoginList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
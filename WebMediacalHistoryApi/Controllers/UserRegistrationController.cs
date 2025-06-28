using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hospital;
using HospitalAPI.Logic;
using System.Data;
using Hospital.UserLogin;
using Hospital.Patient;
using Hospital.staff;
using Microsoft.AspNetCore.Cors;
using Hospital.Doctors;

namespace HospitalAPI.Controllers
{
    [Route("api/[Controller]")]
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

        [HttpPost]
        [Route("SaveUserAndDoctor")]
        public IActionResult SaveUserAndDoctor([FromBody] DoctorLogin user)
        {
            try
            {
                var usermodel = new UserModel
                {
                    Username = user.Username,
                    PasswordHash = user.PasswordHash,
                    Role = user.Role,
                    IsActiveUser = user.IsActive
                };

                var doctors = new Doctor
                {
                    DoctorFullName = user.DoctorFullName,
                    DoctorPhoneNumber = user.DoctorPhoneNumber,
                    DoctorSpecialization = user.DoctorSpecialization,
                    DoctorEmail = user.DoctorEmail,
                    Gender = user.Gender,
                    Qualifications = user.Qualifications,
                    YearsOfExperience = user.YearsOfExperience,
                    LanguagesSpoken = user.LanguagesSpoken,
                    ServicesOffered = user.ServicesOffered,
                    PracticeLocation = user.PracticeLocation,
                    IsMedicallyRegistered = user.IsMedicallyRegistered, 
                    IsActive = user.IsActive
                }; 
                int status = _userDataOperations.SaveDoctorLoginData(usermodel, doctors);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("SaveUserAndPatient")]
        public IActionResult SaveUserAndPatient([FromBody] PatientLogin user)
        {
            try
            {

                var usermodel = new UserModel
                {
                    Username = user.Username,
                    PasswordHash = user.PasswordHash,
                    Role = user.Role,
                    IsActiveUser = user.IsActive
                };


                var patient = new PatientProfile
                {
                    Name = user.Name,
                    DateOfBirth = user.DateOfBirth,
                    ContactDetails = user.ContactDetails,
                    Gender = user.Gender,
                    IsActive =  user.IsActive
                };
                int status = _userDataOperations.SavePatientLoginData(usermodel, patient);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("SaveUserAndStaff")]
        public IActionResult SaveUserAndStaff([FromBody] StaffLogin Stafflog)
        {
            try
            {

                var usermodel = new UserModel
                {
                    Username = Stafflog.Username,
                    PasswordHash = Stafflog.PasswordHash,
                    Role = Stafflog.Role,
                    IsActiveUser = Stafflog.IsActive
                };


                var staffobj = new Staff
                {
                    StaffName = Stafflog.StaffName,
                    StaffContact = Stafflog.StaffContact,
                    Gender = Stafflog.Gender,
                    PatientCount = Stafflog.PatientCount,
                    IsActive = Stafflog.IsActive
                };
                int status = _userDataOperations.SaveStaffLoginData(usermodel, staffobj);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
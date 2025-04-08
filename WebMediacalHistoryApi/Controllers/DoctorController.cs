using Hospital;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebMediacalHistoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorDataOperations _context;

        public DoctorController(DoctorDataOperations context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public IEnumerable<Doctor> GetDoctors()
        {
            return _context.GetAllDetails();
        }

        // GET: api/staff/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor, Staff")]
        public IActionResult GetStaffById(int id)
        {
            var doc1 = _context.QueryData(id); // Use DataOperations to fetch a specific doctor

            if (doc1 == null)
            {
                return NotFound($"Doctor with ID {id} not found."); // Return 404 if not found
            }

            return Ok(doc1); // Return the doctor
        }

        [HttpPost("Add Doctor")]
        [Authorize(Roles = "Staff, Doctor")]
        public IActionResult Create(Doctor doctor)
        {
            var doc = _context.AddData
             (
                doctor.DoctorFullName,
                doctor.DoctorSpecialization,
                doctor.DoctorPhoneNumber,
                doctor.DoctorEmail,
                doctor.Gender,
                doctor.Qualifications,
                doctor.YearsOfExperience,
                doctor.LanguagesSpoken,
                doctor.ServicesOffered,
                doctor.PracticeLocation,
                doctor.IsMedicallyRegistered,
                doctor.IsActive
            );

            if (doc)
            {
                return Ok($"Doctor with name {doctor.DoctorFullName} is added");
            }

            return NotFound("Something went wrong");
        }

        // PUT: api/staff/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff, Doctor")]
        public IActionResult UpdateDoctor(int id, [FromBody] Doctor updateddoc)
        {
            var existingdoc = _context.QueryData(id); // Check if the doctor exists

            if (existingdoc == null)
            {
                return NotFound($"Doctor with ID {id} not found."); // Return 404 if not found
            }

            // Use DataOperations to update the doctor
            var updated = _context.UpdateData(
                id,
                updateddoc.DoctorFullName,
                updateddoc.DoctorSpecialization,
                updateddoc.DoctorPhoneNumber,
                updateddoc.DoctorEmail,
                updateddoc.Gender,
                updateddoc.Qualifications,
                updateddoc.YearsOfExperience,
                updateddoc.LanguagesSpoken,
                updateddoc.ServicesOffered,
                updateddoc.PracticeLocation,
                updateddoc.IsMedicallyRegistered,
                updateddoc.IsActive
            );

            if (updated)
            {
                return NoContent(); // Return 204 No Content
            }

            return NotFound("Something went wrong");
        }

        // DELETE: api/staff/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Doctor")]
        public IActionResult DeleteDoctor(int id)
        {
            var existingStaff = _context.QueryData(id); // Check if the staff exists

            if (existingStaff == null)
            {
                return NotFound($"Doctor with ID {id} not found."); // Return 404 if not found
            }

            _context.DeleteDoctor(id); // Use DataOperations to delete the staff member
            return NoContent(); // Return 204 No Content
        }
    }
}
using Hospital;
using Hospital.Doctors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
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
        [Authorize(Roles = "Patient, Staff, Admin")]
        public IEnumerable<Doctor> GetDoctors()
        {
            return _context.GetAllDetails();
        }

        [HttpPost("Add Doctor")]
        [Authorize(Roles = "Staff, Admin")]
        public IActionResult Create(Doctor doctor)
        {
            var doc = _context.AddData(doctor);
            if (doc)
            {
                return Ok($"Doctor with name {doctor.DoctorFullName} is added");
            }
            return NotFound("Something went wrong");
        }

        // GET: api/DOCTOR/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Patient, Staff, Admin")]
        public IActionResult GetDoctorById(int id)
        {
            var doc1 = _context.QueryData(id); // Use DataOperations to fetch a specific doctor
            if (doc1 == null)
            {
                return NotFound($"Doctor with ID {id} not found."); // Return 404 if not found
            }
            return Ok(doc1); // Return the doctor
        }

        // GET: api/Doctor/specialization/{specialization}
        [HttpGet("specialization/{specialization}")]
        [Authorize(Roles = "Patient, Staff, Admin")]
        public IActionResult GetDoctorBySpecialization(string specialization)
        {
            var doc1 = _context.QueryBySpecialization(specialization);
            if (doc1 == null || doc1.Count == 0)
            {
                return NotFound($"Doctor with Specialization {specialization} not found.");
            }
            return Ok(doc1);
        }

        // GET: api/Doctor/location/{location}

        [HttpGet("location/{location}")]
        [Authorize(Roles = "Patient, Staff, Admin")]
        public IActionResult GetDoctorByLocation(string location)
        {
            var doc1 = _context.QueryByLocation(location);
            if (doc1 == null || doc1.Count == 0)
            {
                return NotFound($"Doctor with Location {location} not found.");
            }
            return Ok(doc1);
        }

        [HttpGet("doctorNamesBySpecialization/{specialization}")]

        [Authorize(Roles = "Patient, Staff, Admin")]

        public IActionResult GetDoctorNamesBySpecialization(string specialization)

        {

            try

            {

                // Fetch doctor names with the given specialization

                var doctorNames = _context.GetAllDetails()

                    .Where(d => d.DoctorSpecialization.Equals(specialization, StringComparison.OrdinalIgnoreCase))

                    .Select(d => d.DoctorFullName) // Select only the names

                    .ToList();

                // Check if no doctor names are found

                if (doctorNames == null || !doctorNames.Any())

                {

                    return NotFound($"No doctors found with specialization '{specialization}'.");

                }

                // Return the list of doctor names

                return Ok(doctorNames);

            }

            catch (Exception ex)

            {

                // Handle any unexpected errors

                return StatusCode(500, $"Internal Server Error: {ex.Message}");

            }

        }

        [HttpGet("specializations")]

        [Authorize(Roles = "Patient, Staff, Admin")]

        public IActionResult GetDistinctSpecializations()

        {

            try

            {

                // Fetch distinct specializations from the Doctor data

                var specializations = _context.GetAllDetails()

                    .Select(d => d.DoctorSpecialization)

                    .Distinct()

                    .ToList();

                // Check if no specializations are found

                if (specializations == null || !specializations.Any())

                {

                    return NotFound("No specializations found.");

                }

                // Return the list of distinct specializations

                return Ok(specializations);

            }

            catch (Exception ex)

            {

                // Handle any unexpected errors

                return StatusCode(500, $"Internal Server Error: {ex.Message}");

            }

        }


        // GET: api/Doctor/experience/{yearsOfExperience}

        [HttpGet("experience/{yearsOfExperience}")]
        [Authorize(Roles = "Doctor, Staff")]
        public IActionResult GetDoctorByExperience(int yearsOfExperience)
        {
            var doc1 = _context.QueryByExperience(yearsOfExperience);
            if (doc1 == null || doc1.Count == 0)
            {
                return NotFound($"Doctor with experience of {yearsOfExperience} not found.");
            }
            return Ok(doc1);
        }

        // GET: api/Doctor/name/{name}

        [HttpGet("name/{name}")]
        [Authorize(Roles = "Doctor, Staff,Patient")]
        public IActionResult GetDoctorByName(string name)
        {
            var doc1 = _context.QueryByName(name);
            if (doc1 == null || doc1.Count == 0)
            {
                return NotFound($"Doctor with name {name} not found.");
            }
            return Ok(doc1);
        }

        // GET: api/Doctor/inactive

        [HttpGet("inactive")]
        [Authorize(Roles = "Doctor, Staff")]
        public IEnumerable<Doctor> GetInactiveDoctors()
        {
            return _context.GetInactiveDoctors();
        }


        // PUT: api/DOCTOR/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor, Staff, Admin")]
        public IActionResult UpdateDoctor(int id, [FromBody] Doctor updateddoc)
        {
            var existingdoc = _context.QueryData(id); // Check if the doctor exists
            if (existingdoc == null)
            {
                return NotFound($"Doctor with ID {id} not found."); // Return 404 if not found
            }
            updateddoc.DoctorID = id;
            var updated = _context.UpdateData(updateddoc);
            if (updated)
            {
                return NoContent(); // Return 204 No Content
            }
            return NotFound("Something went wrong");
        }

        // DELETE: api/DOCTOR/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteDoctor(int id)
        {
            var existingStaff = _context.QueryData(id);
            if (existingStaff == null)
            {
                return NotFound($"Doctor with ID {id} not found."); // Return 404 if not found
            }
            _context.DeleteDoctor(id);
            return NoContent(); // Return 204 No Content
        }
    }
}
using Hospital;
using Hospital.staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
 
namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly DataOperations _dataOperations;
        public StaffController()
        {
            _dataOperations = new DataOperations();
        }

        // GET: api/staff

        [HttpGet]
        [Authorize(Roles = "Staff, Admin")]
        public IActionResult GetAllStaff()
        {
            var staffList = _dataOperations.GetAllStaffDetails();

            return Ok(staffList);

        }

        // GET: api/staff/{id}

        [HttpGet("{id}")]

        [Authorize(Roles = "Staff, Admin")]

        public IActionResult GetStaffById(int id)
        {
            var staff = _dataOperations.QueryStaff(id);

            if (staff == null)

            {
                return NotFound($"Staff member with ID {id} not found.");
            }
            return Ok(staff);

        }

        // GET: api/staff/name/{name}

        [HttpGet("name/{name}")]

        [Authorize(Roles = "Staff, Admin")]

        public IActionResult GetStaffByName(string name)
        {
            var staff = _dataOperations.QueryStaffByName(name);
            if (staff == null)
            {
                return NotFound($"Staff member with name {name} not found.");
            }
            return Ok(staff);
        }

        // POST: api/staff
        [HttpPost]
        [Authorize(Roles = "Staff, Admin")]
        public IActionResult CreateStaff([FromBody] Staff newStaff)
        {
            var result = _dataOperations.AddStaff(
                newStaff.StaffName,
                newStaff.PatientCount,
                newStaff.StaffContact,
                newStaff.Gender,
                newStaff.IsActive
            );
            if (!result)
            {
                return BadRequest("Failed to create staff member.");
            }
            return CreatedAtAction(nameof(GetStaffById), new { id = newStaff.StaffID }, newStaff);
        }

        // PUT: api/staff/{id}

        [HttpPut("{id}")]

        [Authorize(Roles = "Staff, Admin")]
        public IActionResult UpdateStaff(int id, [FromBody] Staff updatedStaff)
        {
            var existingStaff = _dataOperations.QueryStaff(id);
            if (existingStaff == null)
            {
                return NotFound($"Staff member with ID {id} not found.");
            }
            _dataOperations.UpdateStaff(
                id,
                updatedStaff.StaffName,
                updatedStaff.PatientCount,
                updatedStaff.StaffContact,
                updatedStaff.Gender,
                updatedStaff.IsActive
            );
            return NoContent();
        }

        // DELETE: api/staff/{id}

        [HttpDelete("{id}")]

        [Authorize(Roles = "Staff, Admin")]
        public IActionResult DeleteStaff(int id)
        {
            var existingStaff = _dataOperations.QueryStaff(id);
            if (existingStaff == null)
            {
                return NotFound($"Staff member with ID {id} not found.");
            }
            _dataOperations.DeleteStaff(id);
            return NoContent();
        }
    }
}

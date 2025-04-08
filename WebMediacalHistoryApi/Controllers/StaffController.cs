using Hospital;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
 
namespace WebMediacalHistoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly DataOperations _dataOperations;

        public StaffController()
        {
            _dataOperations = new DataOperations(); // Initialize the DataOperations instance
        }

        // GET: api/staff
        [HttpGet]
        [Authorize(Roles = "Staff")]
        public IActionResult GetAllStaff()
        {
            var staffList = _dataOperations.GetAllStaffDetails(); // Use DataOperations to fetch all staff
            return Ok(staffList); // Return the staff list
        }

        // GET: api/staff/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult GetStaffById(int id)
        {
            var staff = _dataOperations.QueryStaff(id); // Use DataOperations to fetch a specific staff member

            if (staff == null)
            {
                return NotFound($"Staff member with ID {id} not found."); // Return 404 if not found
            }

            return Ok(staff); // Return the staff member
        }

        // POST: api/staff
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public IActionResult CreateStaff([FromBody] Staff newStaff)
        {
            var result = _dataOperations.AddStaff(
                newStaff.StaffName,
                newStaff.PatientID,
                newStaff.PatientCount,
                newStaff.IsActive
            ); // Use DataOperations to add a new staff member

            if (!result)
            {
                return BadRequest("Failed to create staff member."); // Return 400 if something went wrong
            }

            return CreatedAtAction(nameof(GetStaffById), new { id = newStaff.StaffID }, newStaff); // Return 201 Created
        }

        // PUT: api/staff/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult UpdateStaff(int id, [FromBody] Staff updatedStaff)
        {
            var existingStaff = _dataOperations.QueryStaff(id); // Check if the staff exists

            if (existingStaff == null)
            {
                return NotFound($"Staff member with ID {id} not found."); // Return 404 if not found
            }

            // Use DataOperations to update the staff member
            _dataOperations.UpdateStaff(
                id,
                updatedStaff.StaffName,
                updatedStaff.PatientID,
                updatedStaff.PatientCount,
                updatedStaff.IsActive
            );

            return NoContent(); // Return 204 No Content
        }

        // DELETE: api/staff/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult DeleteStaff(int id)
        {
            var existingStaff = _dataOperations.QueryStaff(id); // Check if the staff exists

            if (existingStaff == null)
            {
                return NotFound($"Staff member with ID {id} not found."); // Return 404 if not found
            }

            _dataOperations.DeleteStaff(id); // Use DataOperations to delete the staff member
            return NoContent(); // Return 204 No Content
        }
    }
}

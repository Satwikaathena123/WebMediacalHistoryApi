using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hospital;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Hospital.Appointment;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly BLLAppointment _bllAppointments;
        private int id;

        // Constructor to initialize BLLCustomer
        public AppointmentController()
        {
            _bllAppointments = new BLLAppointment();
        }
        [HttpGet]
        [Authorize(Roles = "Patient, Admin")]
        public async Task<ActionResult<IEnumerable<AppointmentHistory>>> GetAppointments()
        {
            try
            {
                // Fetch appointment data from BLLCustomer
                DataTable dataTable = await Task.Run(() => _bllAppointments.spFillAppointments());

                // Check if data is empty
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return NotFound("No appointments found in the database.");
                }

                // Convert DataTable to a List of AppointmentHistory objects
                List<AppointmentHistory> appointmentsList = _bllAppointments.AppointmentConvertDataTableToList(dataTable);

                // Return the list of appointments as JSON
                return Ok(appointmentsList);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return a 500 status code
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{appointmentid}")]
        [Authorize(Roles = "Patient, Admin")]
        public async Task<ActionResult<AppointmentHistory>> GetAppointmentById(int appointmentid)
        {
            try
            {
                // Fetch all appointments and filter by AppointmentID
                DataTable dataTable = await Task.Run(() => _bllAppointments.spFetchAppointmentsData(appointmentid));

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return NotFound("No appointments found in the database.");
                }

                // Convert DataTable to a list of AppointmentHistory objects
                var appointmentsList = _bllAppointments.AppointmentConvertDataTableToList(dataTable);

                // Find the appointment with the specified ID


                if (appointmentsList == null)
                {
                    return NotFound($"Appointment with ID {appointmentid} not found.");
                }

                // Return the appointment details
                return Ok(appointmentsList);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a 500 Internal Server Error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Patient, Admin")]
        public async Task<ActionResult> CreateAppointment([FromBody] AppointmentHistory newAppointment)
        {
            try
            {
                // Call the BLL method to save the new appointment
                int result = await Task.Run(() => _bllAppointments.spSaveAppointment(newAppointment));

                if (result > 0)
                {
                    return Ok("Appointment created successfully.");
                }
                else
                {
                    return BadRequest("Failed to create appointment.");
                }
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error in case of exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // PUT: api/Appointments/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Patient, Admin")]
        public async Task<ActionResult> UpdateAppointment(int id, [FromBody] AppointmentHistory updatedAppointment)
        {
            try
            {
                // Ensure the AppointmentID matches the ID from the route parameter
                updatedAppointment.AppointmentID = id;

                // Call the BLL method to update the appointment
                int result = await Task.Run(() => _bllAppointments.UpdateAppointment(updatedAppointment));

                if (result > 0)
                {
                    return Ok("Appointment updated successfully.");
                }
                else
                {
                    return NotFound($"Appointment with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a 500 status code
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{appointmentID}")]
        [Authorize(Roles = "Patient, Admin")]

        public async Task<ActionResult> DeleteAppointment(int appointmentID, [FromBody] AppointmentHistory updatedHistory)

        {

            try

            {

                // Call the BLL method to delete the appointment

                updatedHistory.AppointmentID = appointmentID;

                int result = await Task.Run(() => _bllAppointments.spDeleteAppointment(updatedHistory));

                if (result > 0)

                {

                    return Ok("Appointment deleted successfully.");

                }

                else

                {

                    return NotFound($"Appointment with ID {updatedHistory.AppointmentID} not found.");

                }

            }

            catch (Exception ex)

            {

                // Handle exceptions and return a 500 Internal Server Error

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }


    }
}
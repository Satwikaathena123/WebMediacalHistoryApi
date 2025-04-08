using Hospital;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data; // Add this line

namespace WebMediacalHistoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentHistoryController : ControllerBase
    {
        private readonly BLLAppointments _bLLAppointments;
        private int id;

        // Constructor to initialize BLLCustomer
        public AppointmentHistoryController()
        {
            _bLLAppointments = new BLLAppointments();
        }

        [HttpGet]
        [Authorize(Roles = "Staff")]
        public ActionResult<IEnumerable<AppointmentHistory>> GetAppointments()
        {
            try
            {
                // Fetch appointment data from BLLCustomer
                DataTable dataTable = _bLLAppointments.FillAppointments();

                // Check if data is empty
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return NotFound("No appointments found in the database.");
                }

                // Convert DataTable to a List of AppointmentHistory objects
                List<AppointmentHistory> appointmentsList = _bLLAppointments.AppointmentConvertDataTableToList(dataTable);

                // Return the list of appointments as JSON
                return Ok(appointmentsList);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return a 500 status code
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{AppointmentID}")]
        [Authorize(Roles = "Staff")]
        public ActionResult<AppointmentHistory> GetAppointmentById(int AppointmentID)
        {
            try
            {
                // Fetch all appointments and filter by AppointmentID
                DataTable dataTable = _bLLAppointments.FillAppointments();

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return NotFound("No appointments found in the database.");
                }

                // Convert DataTable to a list of AppointmentHistory objects
                var appointmentsList = _bLLAppointments.AppointmentConvertDataTableToList(dataTable);

                // Find the appointment with the specified ID
                var appointment = appointmentsList.Find(a => a.AppointmentID == AppointmentID);

                if (appointment == null)
                {
                    return NotFound($"Appointment with ID {AppointmentID} not found.");
                }

                // Return the appointment details
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a 500 Internal Server Error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public ActionResult CreateAppointment([FromBody] AppointmentHistory newAppointment)
        {
            try
            {
                // Call the BLL method to save the new appointment
                int result = _bLLAppointments.SaveAppointment(newAppointment);

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
        [Authorize(Roles = "Staff")]
        public ActionResult UpdateAppointment(int id, [FromBody] AppointmentHistory updatedAppointment)
        {
            try
            {
                // Ensure the AppointmentID matches the ID from the route parameter
                updatedAppointment.AppointmentID = id;

                // Call the BLL method to update the appointment
                int result = _bLLAppointments.UpdateAppointment(updatedAppointment);

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
        [Authorize(Roles = "Staff")]
        public ActionResult DeleteAppointment(int appointmentID, [FromBody] AppointmentHistory updatedHistory)
        {
            try
            {
                // Call the BLL method to delete the appointment
                updatedHistory.AppointmentID = appointmentID;
                int result = _bLLAppointments.DeleteAppointment(updatedHistory);

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

using Hospital;
using Hospital;
using Hospital.DoctorSchedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Optional: remove if public access needed
    public class DoctorScheduleController : ControllerBase
    {
        private readonly HandlingMethods _handler;

        public DoctorScheduleController()
        {
            _handler = new HandlingMethods();
        }

        [Authorize(Roles = "Admin, Staff")]
        [HttpPost("AddDoctor")]
        public IActionResult AddDoctor(string name, List<TimeSlotDto> timeSlots)
        {
            var result = _handler.AddDoctors(name, timeSlots);
            return result ? Ok("Doctor added successfully.") : BadRequest("Failed to add doctor.");
        }

        [Authorize(Roles = "Admin, Staff")]
        [HttpPut("UpdateDoctorTimeSlots/{id}")]
        public IActionResult UpdateDoctorTimeSlots(int id, List<TimeSlot> timeSlots)
        {
            var result = _handler.UpdateDoctorTimeSlots(id, timeSlots);
            return result ? Ok("Doctor time slots updated.") : NotFound("Doctor not found.");
        }


        [Authorize(Roles = "Admin, Staff")]
        [HttpPut("UpdateDoctorTimeSlotForDay/{id}")]
        public IActionResult UpdateDoctorTimeSlotForDay(int id, string day, List<string> newSlots)
        {
            var result = _handler.UpdateDoctorTimeSlotForDay(id, day, newSlots);
            return result ? Ok("Time slots updated for the day.") : NotFound("Doctor not found.");
        }

        [Authorize(Roles = "Admin, Staff")]
        [HttpDelete("RemoveDoctor/{id}")]
        public IActionResult RemoveDoctor(int id)
        {
            var result = _handler.RemoveDoctorFromSchedule(id);
            return result ? Ok("Doctor removed successfully.") : NotFound("Doctor not found.");
        }

        [Authorize(Roles = "Admin,Patient,Staff")]
        [HttpGet("GetDoctorById/{id}")]
        public IActionResult GetDoctorById(int id)
        {
            var doctor = _handler.GetDoctorDetailsById(id);
            return doctor != null ? Ok(doctor) : NotFound("Doctor not found.");
        }

        [HttpGet("GetDoctorSlotsByDay/{id}/{day}")]
        [Authorize(Roles = "Admin, Staff, Patient")]
        public IActionResult GetDoctorSlotsByDay(int id, string day)
        {
            try
            {
                // Fetch the doctor details by ID
                var doctor = _handler.GetDoctorDetailsById(id);

                // Check if the doctor exists
                if (doctor == null)
                {
                    return NotFound($"Doctor with ID {id} not found.");
                }

                // Filter the time slots for the given day
                var slotsForDay = doctor.AvailableTimeSlots
                    .Where(slot => slot.Day.Equals(day, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(slot => slot.Slot)
                    .ToList();

                // Check if no slots are found for the given day
                if (slotsForDay == null || !slotsForDay.Any())
                {
                    return NotFound($"No slots found for Doctor with ID {id} on {day}.");
                }

                // Return the list of slots for the given day
                return Ok(slotsForDay);
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }



        [HttpGet("GetDoctorByName")]
        [Authorize(Roles = "Patient,Admin,Staff")]
        public IActionResult GetDoctorByName(string name)
        {
            var doctor = _handler.GetDoctorDetailsByName(name);
            return doctor != null ? Ok(doctor) : NotFound("Doctor not found.");
        }


        [HttpGet("GetAllDoctors")]
        [Authorize(Roles = "Admin,Patient,Staff")]

        public IActionResult GetAllDoctors()
        {
            var doctors = _handler.GetAllDoctors();
            return Ok(doctors);
        }
    }
}


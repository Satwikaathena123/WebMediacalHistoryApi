using Hospital;
using Hospital;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebMediacalHistoryApi.Controllers
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

        [Authorize(Roles = "Admin")]
        [HttpPost("AddDoctor")]
        public IActionResult AddDoctor(string name, List<TimeSlotDto> timeSlots)
        {
            var result = _handler.AddDoctors(name, timeSlots);
            return result ? Ok("Doctor added successfully.") : BadRequest("Failed to add doctor.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateDoctorTimeSlots/{id}")]
        public IActionResult UpdateDoctorTimeSlots(int id, List<TimeSlot> timeSlots)
        {
            var result = _handler.UpdateDoctorTimeSlots(id, timeSlots);
            return result ? Ok("Doctor time slots updated.") : NotFound("Doctor not found.");
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateDoctorTimeSlotForDay/{id}")]
        public IActionResult UpdateDoctorTimeSlotForDay(int id, string day, List<string> newSlots)
        {
            var result = _handler.UpdateDoctorTimeSlotForDay(id, day, newSlots);
            return result ? Ok("Time slots updated for the day.") : NotFound("Doctor not found.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("RemoveDoctor/{id}")]
        public IActionResult RemoveDoctor(int id)
        {
            var result = _handler.RemoveDoctorFromSchedule(id);
            return result ? Ok("Doctor removed successfully.") : NotFound("Doctor not found.");
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpGet("GetDoctorById/{id}")]
        public IActionResult GetDoctorById(int id)
        {
            var doctor = _handler.GetDoctorDetailsById(id);
            return doctor != null ? Ok(doctor) : NotFound("Doctor not found.");
        }



        [HttpGet("GetDoctorByName")]
        [Authorize(Roles = "Patient,Admin")]
        public IActionResult GetDoctorByName(string name)
        {
            var doctor = _handler.GetDoctorDetailsByName(name);
            return doctor != null ? Ok(doctor) : NotFound("Doctor not found.");
        }


        [HttpGet("GetAllDoctors")]
        [Authorize(Roles = "Admin,Patient")]

        public IActionResult GetAllDoctors()
        {
            var doctors = _handler.GetAllDoctors();
            return Ok(doctors);
        }
    }
}


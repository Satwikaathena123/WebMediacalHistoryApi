using Hospital;
using Hospital.Medical_History;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly BLLMedicalHistory _bllhistory;

        public MedicalHistoryController()
        {
            _bllhistory = new BLLMedicalHistory();
        }

        //[HttpGet]
        //public ActionResult<IEnumerable<MedicalHistory>> GetHistoryEntries()
        //{
        //    //Converter conv = new();
        //    DataTable dt = _bllhistory.FetchPatientsData();
        //    var patientList = _bllhistory.HistoryConvertDataTableToList(dt);
        //    return Ok(patientList);
        //}

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public ActionResult<IEnumerable<MedicalHistory>> GetActiveHistoryEntries()
        {
            //Converter conv = new();
            DataTable dt = _bllhistory.spFetchActivePatientsData();
            var patientList = _bllhistory.HistoryConvertDataTableToList(dt);
            return Ok(patientList);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Patient")]
        public ActionResult<IEnumerable<MedicalHistory>> GetSelectedHistoryEntries(int id)
        {
            DataTable dt = _bllhistory.spFetechSingleTable(id);
            var patientList = _bllhistory.HistoryConvertDataTableToList(dt);
            return Ok(patientList);
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public ActionResult PutHistoryEntries([FromBody] MedicalHistory patientHistory)
        {
            try
            {
                int result = _bllhistory.SavePatient(patientHistory);

                if (result == 0)
                {
                    return StatusCode(500, $"Updated Successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server error: {ex.Message}");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Patient")]
        public ActionResult UpdateHistoryEntries(int id, [FromBody] MedicalHistory updatedHistory)
        {
            try
            {
                updatedHistory.HistoryID = id;
                int result = _bllhistory.spUpdatePatient(updatedHistory);

                if (result > 0)
                {
                    return Ok("Updated");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Patient")]
        public ActionResult DeleteHistory(int id, [FromBody] MedicalHistory updatedHistory)
        {
            try
            {
                updatedHistory.HistoryID = id;
                int result = _bllhistory.spDeletePatientHistory(updatedHistory);

                if (result > 0)
                {
                    return Ok("Deleted");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


    }
}

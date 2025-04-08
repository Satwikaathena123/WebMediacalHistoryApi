using Hospital;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;

namespace WebMediacalHistoryApi.Controllers
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
        //public ActionResult<IEnumerable<PatientHistory>> GetHistoryEntries()
        //{
        //    //Converter conv = new();
        //    DataTable dt = _bllhistory.FetchPatientsData();
        //    var patientList = _bllhistory.ConvertDataTableToList(dt);
        //    return Ok(patientList);
        //}

        [HttpGet]
        [Authorize(Roles = "Doctor, Patient")]
        public ActionResult<IEnumerable<MedicalHistory>> GetActiveHistoryEntries()
        {
            try
            {
                DataTable dt = _bllhistory.FetchActivePatientsData();
                if (dt == null || dt.Rows.Count == 0)
                {
                    return NotFound("No History found in the database.");
                }
                var patientList = _bllhistory.HistoryConvertDataTableToList(dt);
                return Ok(patientList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor, Patient")]
        public ActionResult<IEnumerable<MedicalHistory>> GetSelectedHistoryEntries(int id)
        {   
            try
            {
                DataTable dt = _bllhistory.FetechSingleTable(id);
                
                if(dt == null || dt.Rows.Count == 0)
                {
                    return NotFound("No History Found.");
                }
                var patientList = _bllhistory.HistoryConvertDataTableToList(dt);
                
                if (patientList == null)
                {
                    return NotFound($"History with ID {id} not found.");
                }
                return Ok(patientList);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
           
        }

        [HttpPost]
        [Authorize(Roles = "Doctor, Patient")]
        public ActionResult PutHistoryEntries([FromBody] MedicalHistory patientHistory)
        {
            try
            {
                int result = _bllhistory.SavePatient(patientHistory);

                if (result == 0)
                {
                    return StatusCode(500, $"Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to create History.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server error: {ex.Message}");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor, Patient")]
        public ActionResult UpdateHistoryEntries(int id, [FromBody]MedicalHistory updatedHistory)
        {
            try
            {
                updatedHistory.HistoryID = id;
                int result = _bllhistory.UpdatePatient(updatedHistory);

                if (result >0)
                {
                    return Ok("Updated Successfully.");
                }
                else
                {
                    return NotFound($"History with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor, Patient")]
        public ActionResult DeleteHistory(int id, [FromBody] MedicalHistory updatedHistory)
        {
            try
            {
                updatedHistory.HistoryID = id;
                int result = _bllhistory.DeletePatientHistory(updatedHistory);

                if (result > 0)
                {
                    return Ok("Deleted Successfully.");
                }
                else
                {
                    return NotFound($"History with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

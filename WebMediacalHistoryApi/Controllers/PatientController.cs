using Hospital;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace WebMediacalHistoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly BLLPatient _bllPatient;

        public PatientController()
        {
            _bllPatient = new BLLPatient();
        }

        // GET: api/Patient
        [HttpGet]
        [Authorize(Roles = "Staff, Patient")]
        public ActionResult<IEnumerable<PatientProfile>> GetData()
        {
            try
            {
                var dataTable = _bllPatient.FetchActivePatients();

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return NotFound("No patients found in the database.");
                }

                var patientsList = _bllPatient.ConvertDataTableToList(dataTable);

                return Ok(patientsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Patient/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff, Patient")]
        public ActionResult<PatientProfile> GetPatientById(int id)
        {
            try
            {
                var dataTable = _bllPatient.FillPatients();

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }

                var patientsList = _bllPatient.ConvertDataTableToList(dataTable);
                var patient = patientsList.Find(p => p.PatientID == id);

                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Patient
        [HttpPost]
        [Authorize(Roles = "Staff, Patient")]
        public ActionResult CreatePatient([FromBody] PatientProfile newPatient)
        {
            try
            {
                int result = _bllPatient.SavePatient(newPatient);

                if (result > 0)
                {
                    return Ok("Patient created successfully.");
                }
                else
                {
                    return BadRequest("Failed to create patient.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Patient/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff, Patient")]
        public ActionResult UpdatePatient(int id, [FromBody] PatientProfile updatedPatient)
        {
            try
            {
                updatedPatient.PatientID = id;
                int result = _bllPatient.UpdatePatient(updatedPatient);

                if (result > 0)
                {
                    return Ok("Patient updated successfully.");
                }
                else
                {
                    return NotFound($"Patient with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Patient/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff, Patient")]
        public ActionResult DeletePatient(int id)
        {
            try
            {
                int result = _bllPatient.DeletePatient(id);

                if (result > 0)
                {
                    return Ok("Patient deleted successfully.");
                }
                else
                {
                    return NotFound($"Patient with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

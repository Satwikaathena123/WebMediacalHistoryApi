using Hospital;
using Hospital.Medical_History;
using Hospital.Patient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using System.Data;

namespace HospitalAPI.Controllers

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

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<ActionResult<IEnumerable<PatientProfile>>> GetData()

        {

            try

            {

                var dataTable = await Task.Run(() => _bllPatient.spFetchActivePatients());

                if (dataTable == null || dataTable.Rows.Count == 0)

                {

                    return NotFound("No patients found in the database.");

                }

                var patientsList = _bllPatient.PatientConvertDataTableToList(dataTable);

                return Ok(patientsList);

            }

            catch (Exception ex)

            {

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }

        // GET: api/Patient/{id}

        [HttpGet("{id}")]

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<ActionResult<IEnumerable<PatientProfile>>> GetPatientById(int id)

        {

            try

            {

                DataTable dt = await Task.Run(() => _bllPatient.spFetchSingleTable(id));

                if (dt == null || dt.Rows.Count == 0)

                {

                    return NotFound("No Patient found.");

                }

                var patientList = _bllPatient.PatientConvertDataTableToList(dt);

                if (patientList == null)

                {

                    return NotFound($"Patient with ID {id} not found.");

                }

                return Ok(patientList);

            }

            catch (Exception ex)

            {

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }

        // GET: api/Patient/name/{patientName}

        [HttpGet("name/{patientName}")]

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<ActionResult<IEnumerable<PatientProfile>>> GetPatientByName(string patientName)

        {

            try

            {

                DataTable dt = await Task.Run(() => _bllPatient.FetchPatientByName(patientName));

                if (dt == null || dt.Rows.Count == 0)

                {

                    return NotFound("No Patient found.");

                }

                var patientList = _bllPatient.PatientConvertDataTableToList(dt);

                if (patientList == null)

                {

                    return NotFound($"Patient with name {patientName} not found.");

                }

                return Ok(patientList);

            }

            catch (Exception ex)

            {

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }


        [HttpGet]

        [Route("GetAllActivePatientHistories")]

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<ActionResult<IEnumerable<object>>> GetActivePatientHistories()

        {

            try

            {

                // Call the BLL method to fetch data from the stored procedure

                var result = await Task.Run(() => _bllPatient.spFetechActivePatientWithMedicalHistory());

                if (result == null)

                {

                    return NotFound($"No active patients found.");

                }

                // Return the result as a JSON response

                return Ok(result);

            }

            catch (Exception ex)

            {

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }

        [HttpGet]

        [Route("GetActivePatientHistoryWithId/{id}")]

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<ActionResult<IEnumerable<object>>> GetActivePatientHistoryWithId(int id)

        {

            try

            {

                // Call the BLL method to fetch data from the stored procedure

                var result = await Task.Run(() => _bllPatient.spFetchActivePatientHistoryWithId(id));

                if (result == null || result.Count == 0)

                {

                    return NotFound($"No active patient found with ID {id}.");

                }

                // Return the result as a JSON response

                return Ok(result);

            }

            catch (Exception ex)

            {

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }


        // POST: api/Patient

        [HttpPost]

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<ActionResult> CreatePatient([FromBody] PatientProfile newPatient)

        {

            try

            {

                int result = await Task.Run(() => _bllPatient.spSavePatients(newPatient));

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

        [HttpPost]

        [Route("AddPatientWithMedicalHistory")]

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<IActionResult> AddPatientWithMedicalHistory(

    [FromBody] PatientProfile patientProfile,

    [FromQuery] MedicalHistory medicalHistory)

        {

            if (patientProfile == null || string.IsNullOrWhiteSpace(patientProfile.Name))

                return BadRequest("Invalid patient profile payload.");

            try

            {

                Console.WriteLine($"Name: {patientProfile.Name}"); // Debugging

                var result = await Task.Run(() =>

                    _bllPatient.SavePatientsAndMedicalHistory(patientProfile, medicalHistory));

                if (result > 0)

                    return Ok("Patient and medical history added successfully.");

                else

                    return BadRequest("Failed to add patient and medical history.");

            }

            catch (Exception ex)

            {

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }



        // PUT: api/Patient/{id}

        [HttpPut("{id}")]

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<ActionResult> UpdatePatient(int id, [FromBody] PatientProfile updatedPatient)

        {

            try

            {

                updatedPatient.PatientID = id;

                int result = await Task.Run(() => _bllPatient.spUpdatePatients(updatedPatient));

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

        [Authorize(Roles = "Patient,Staff,Admin")]

        public async Task<ActionResult> DeletePatient(int id)

        {

            try

            {

                int result = await Task.Run(() => _bllPatient.spDeletePatient(id));

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


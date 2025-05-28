using apbd12c_cw11.DTOs;
using apbd12c_cw11.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd12c_cw11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public PrescriptionsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionRequestDto requestDto)
        {
            try
            {
                var prescriptionId = await _dbService.CreatePrescriptionAsync(requestDto);
                return Ok(new { IdPrescription = prescriptionId, Message = "Utworzono nową receptę." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetPatientDetails(int patientId)
        {
            try
            {
                var patient = await _dbService.GetPatientDetailsAsync(patientId);
                return Ok(patient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
using apbd12c_cw11.DTOs;

namespace apbd12c_cw11.Services;

public interface IDbService
{
    Task<int> CreatePrescriptionAsync(CreatePrescriptionRequestDto requestDto);
    Task<PatientDetailsResponseDto> GetPatientDetailsAsync(int patientId);
}
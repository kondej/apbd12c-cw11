namespace apbd12c_cw11.DTOs;

public class PatientDetailsResponseDto
{
    public required int IdPatient { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }
    public required List<PrescriptionDto> Prescriptions { get; set; }
}
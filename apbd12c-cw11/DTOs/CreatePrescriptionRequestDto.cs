namespace apbd12c_cw11.DTOs;

public class CreatePrescriptionRequestDto
{
    public required PatientDto Patient { get; set; }
    public required int IdDoctor { get; set; }
    public required List<MedicamentRequestDto> Medicaments { get; set; }
    public required DateTime Date { get; set; }
    public required DateTime DueDate { get; set; }
}
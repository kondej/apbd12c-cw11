namespace apbd12c_cw11.DTOs;

public class PrescriptionDto
{
    public required int IdPrescription { get; set; }
    public required DateTime Date { get; set; }
    public required DateTime DueDate { get; set; }
    public required List<MedicamentDto> Medicaments { get; set; }
    public required DoctorDto Doctor { get; set; }
}
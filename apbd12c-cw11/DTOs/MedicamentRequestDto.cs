namespace apbd12c_cw11.DTOs;

public class MedicamentRequestDto
{
    public required int IdMedicament { get; set; }
    public int Dose { get; set; }
    public required string Description { get; set; }
}
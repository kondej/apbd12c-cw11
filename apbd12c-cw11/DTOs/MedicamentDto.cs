namespace apbd12c_cw11.DTOs;

public class MedicamentDto
{
    public required int IdMedicament { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Type { get; set; }
    public int Dose { get; set; }
    public required string Details { get; set; }
}
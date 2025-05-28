using System.ComponentModel.DataAnnotations;

namespace apbd12c_cw11.DTOs;

public class PatientDto
{
    public required int IdPatient { get; set; }
    [MaxLength(100)] 
    public required string FirstName { get; set; }
    [MaxLength(100)]
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }
}
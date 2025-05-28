using System.ComponentModel.DataAnnotations;

namespace apbd12c_cw11.Models;

public class Patient
{
    [Key]
    public int IdPatient { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    public DateTime BirthDate { get; set; }
        
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
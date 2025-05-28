using System.ComponentModel.DataAnnotations;

namespace apbd12c_cw11.Models;

public class Doctor
{
    [Key]
    public int IdDoctor { get; set; }

    [Required]
    [MaxLength(100)] 
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
        
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
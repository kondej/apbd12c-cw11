using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apbd12c_cw11.Models;

public class PrescriptionMedicament
{
    [ForeignKey(nameof(Prescription))]
    public int IdPrescription { get; set; }
    
    [Required]
    public Prescription Prescription { get; set; } = null!;
        
    [ForeignKey(nameof(Medicament))]
    public int IdMedicament { get; set; }
    
    [Required]
    public Medicament Medicament { get; set; } = null!;
    
    public int Dose { get; set; }
        
    [Required]
    [MaxLength(100)]
    public string Details { get; set; } = string.Empty;
}
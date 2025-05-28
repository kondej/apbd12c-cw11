using apbd12c_cw11.Data;
using apbd12c_cw11.DTOs;
using apbd12c_cw11.Models;
using Microsoft.EntityFrameworkCore;

namespace apbd12c_cw11.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;
    public DbService(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<int> CreatePrescriptionAsync(CreatePrescriptionRequestDto requestDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var doctor = await _context.Doctors.AnyAsync(d => d.IdDoctor == requestDto.IdDoctor);
            if (!doctor)
            {
                throw new ArgumentException($"Doktor o id {requestDto.IdDoctor} nie istnieje!");
            }
            
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdPatient == requestDto.Patient.IdPatient);

            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = requestDto.Patient.FirstName,
                    LastName = requestDto.Patient.LastName,
                    BirthDate = requestDto.Patient.BirthDate
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            var medicaments = requestDto.Medicaments.Select(m => m.IdMedicament).ToList();
            var existingMedicaments = await _context.Medicaments
                .Where(m => medicaments.Contains(m.IdMedicament))
                .Select(m => m.IdMedicament)
                .ToListAsync();

            var missingMedicaments = medicaments.Except(existingMedicaments).ToList();
            if (missingMedicaments.Any())
            {
                throw new ArgumentException($"Medykamenty o id {string.Join(", ", missingMedicaments)} nie istnieją.");
            }

            if (requestDto.Medicaments.Count > 10)
                throw new InvalidOperationException("Recepta może zawierać maksymalnie 10 leków!");

            if (requestDto.DueDate < requestDto.Date)
                throw new InvalidOperationException("DueDate musi być większe lub równe Date.");

            var prescription = new Prescription
            {
                Date = requestDto.Date,
                DueDate = requestDto.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor = requestDto.IdDoctor
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            var prescriptionMedicaments = requestDto.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdPrescription = prescription.IdPrescription,
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Description
            }).ToList();

            _context.PrescriptionMedicaments.AddRange(prescriptionMedicaments);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return prescription.IdPrescription;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PatientDetailsResponseDto> GetPatientDetailsAsync(int patientId)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions
                .OrderBy(pr => pr.DueDate))
            .ThenInclude(pr => pr.Doctor)
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .FirstOrDefaultAsync(p => p.IdPatient == patientId);

        if (patient == null)
            throw new KeyNotFoundException($"Pacjent o id {patientId} nie istnieje!");

        return new PatientDetailsResponseDto
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            BirthDate = patient.BirthDate,
            Prescriptions = patient.Prescriptions.Select(p => new PrescriptionDto
            {
                IdPrescription = p.IdPrescription,
                Date = p.Date,
                DueDate = p.DueDate,
                Doctor = new DoctorDto
                {
                    IdDoctor = p.Doctor.IdDoctor,
                    FirstName = p.Doctor.FirstName,
                    LastName = p.Doctor.LastName,
                    Email = p.Doctor.Email
                },
                Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentDto
                {
                    IdMedicament = pm.Medicament.IdMedicament,
                    Name = pm.Medicament.Name,
                    Description = pm.Medicament.Description,
                    Type = pm.Medicament.Type,
                    Dose = pm.Dose,
                    Details = pm.Details
                }).ToList()
            }).ToList()
        };
    }
}
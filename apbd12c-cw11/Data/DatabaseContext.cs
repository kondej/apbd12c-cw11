using apbd12c_cw11.Models;
using Microsoft.EntityFrameworkCore;

namespace apbd12c_cw11.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.IdDoctor);
            entity.Property(e => e.IdDoctor).ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
        });
        
        modelBuilder.Entity<Medicament>(entity =>
        {
            entity.HasKey(e => e.IdMedicament);
            entity.Property(e => e.IdMedicament).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(100);
        });
        
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.IdPatient);
            entity.Property(e => e.IdPatient).ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.BirthDate).HasColumnType("date");
        });
        
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.IdPrescription);
            entity.Property(e => e.IdPrescription).ValueGeneratedOnAdd();
            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.DueDate).HasColumnType("date");

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(e => e.IdPatient)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(e => e.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<PrescriptionMedicament>(entity =>
        {
            entity.HasKey(e => new { e.IdPrescription, e.IdMedicament });

            entity.HasOne(e => e.Prescription)
                .WithMany(p => p.PrescriptionMedicaments)
                .HasForeignKey(e => e.IdPrescription)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Medicament)
                .WithMany(m => m.PrescriptionMedicaments)
                .HasForeignKey(e => e.IdMedicament)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Dose);
            entity.Property(e => e.Details).HasMaxLength(100);
        });
        
        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                BirthDate = new DateTime(1980, 4, 15),
            },
            new Patient
            {
                IdPatient = 2,
                FirstName = "Anna",
                LastName = "Nowak",
                BirthDate = new DateTime(1984, 7, 20),
            }
        );
        
        modelBuilder.Entity<Doctor>().HasData(new Doctor
        {
            IdDoctor = 1,
            FirstName = "Jacek",
            LastName = "Kowal",
            Email = "jacek.kowal@clinic.pl"
        });

        modelBuilder.Entity<Medicament>().HasData(
            new Medicament
            {
                IdMedicament = 1,
                Name = "Ibuprofen",
                Description = "Przeciwbolowe",
                Type = "Tabletki"
            },
            new Medicament
            {
                IdMedicament = 2,
                Name = "Paracetamol",
                Description = "Przeciwzapalne",
                Type = "Tabletki"
            },
            new Medicament
            {
                IdMedicament = 3,
                Name = "Amoxicillin",
                Description = "Antybiotyk",
                Type = "Kapsulki"
            }
        );


        modelBuilder.Entity<Prescription>().HasData(
            new Prescription
            {
                IdPrescription = 1,
                IdPatient = 1,
                IdDoctor = 1,
                Date = new DateTime(2023, 10, 1),
                DueDate = new DateTime(2023, 11, 1),
            }
        );

        modelBuilder.Entity<PrescriptionMedicament>().HasData(
            new PrescriptionMedicament
            {
                IdPrescription = 1,
                IdMedicament = 1,
                Dose = 2,
                Details = "2x dziennie po jedzeniu"
            },
            new PrescriptionMedicament
            {
                IdPrescription = 1,
                IdMedicament = 2,
                Dose = 1,
                Details = "Na noc przez 5 dni"
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}
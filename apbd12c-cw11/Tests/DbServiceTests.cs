using apbd12c_cw11.Controllers;
using apbd12c_cw11.DTOs;
using apbd12c_cw11.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace apbd12c_cw11.Tests;

public class DbServiceTests
{
    private readonly Mock<IDbService> _mockService;
    private readonly PrescriptionsController _controller;
    
    public DbServiceTests()
    {
        _mockService = new Mock<IDbService>();
        _controller = new PrescriptionsController(_mockService.Object);
    }

    [Fact]
    public async Task CreatePrescription_For_ValidRequest()
    {
        var request = new CreatePrescriptionRequestDto
        {
            IdDoctor = 1,
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Patient = new PatientDto
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                BirthDate = DateTime.Parse("1990-01-01")
            },
            Medicaments = new List<MedicamentRequestDto>
            {
                new MedicamentRequestDto
                {
                    IdMedicament = 1,
                    Dose = 2,
                    Description = "2x dziennie"
                }
            }
        };
        
        var expectedPrescriptionId = 123;
        _mockService.Setup(s => s.CreatePrescriptionAsync(request))
            .ReturnsAsync(expectedPrescriptionId);
        var result = await _controller.CreatePrescription(request);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;

        var id = (int?) response?.GetType().GetProperty("IdPrescription")?.GetValue(response);
        var message = response?.GetType().GetProperty("Message")?.GetValue(response) as string;

        Assert.NotNull(id);
        Assert.NotNull(message);
        Assert.Equal(expectedPrescriptionId, id);
        Assert.Equal("Utworzono nową receptę.", message);
    }
    
    [Fact]
    public async Task CreatePrescription_For_InvalidDoctor()
    {
        var request = new CreatePrescriptionRequestDto
        {
            IdDoctor = 999,
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Patient = new PatientDto
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                BirthDate = DateTime.Parse("1990-01-01")
            },
            Medicaments = new List<MedicamentRequestDto>()
        };

        var errorMessage = "Doktor o id 999 nie istnieje!";
        _mockService.Setup(s => s.CreatePrescriptionAsync(request))
            .ThrowsAsync(new ArgumentException(errorMessage));
        
        var result = await _controller.CreatePrescription(request);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
    
    [Fact]
    public async Task CreatePrescription_For_ExceededMedicaments()
    {
        var medicaments = new List<MedicamentRequestDto>();
        for (int i = 1; i <= 11; i++)
        {
            medicaments.Add(new MedicamentRequestDto
            {
                IdMedicament = i,
                Dose = 1,
                Description = "1x dziennie"
            });
        }

        var request = new CreatePrescriptionRequestDto
        {
            IdDoctor = 1,
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Patient = new PatientDto
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                BirthDate = DateTime.Parse("1990-01-01")
            },
            Medicaments = medicaments
        };

        var errorMessage = "Recepta może zawierać maksymalnie 10 leków!";
        _mockService.Setup(s => s.CreatePrescriptionAsync(request))
            .ThrowsAsync(new InvalidOperationException(errorMessage));
        
        var result = await _controller.CreatePrescription(request);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
    
    [Fact]
    public async Task CreatePrescription_For_InvalidDueDate()
    {
        var request = new CreatePrescriptionRequestDto
        {
            IdDoctor = 1,
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(-1),
            Patient = new PatientDto
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                BirthDate = DateTime.Parse("1990-01-01")
            },
            Medicaments = new List<MedicamentRequestDto>
            {
                new MedicamentRequestDto
                {
                    IdMedicament = 1,
                    Dose = 1,
                    Description = "1x dziennie"
                }
            }
        };

        var errorMessage = "DueDate musi być większe lub równe Date.";
        _mockService.Setup(s => s.CreatePrescriptionAsync(request))
            .ThrowsAsync(new InvalidOperationException(errorMessage));
        
        var result = await _controller.CreatePrescription(request);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
    
    [Fact]
    public async Task CreatePrescription_For_NonExistentMedicaments()
    {
        var request = new CreatePrescriptionRequestDto
        {
            IdDoctor = 1,
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Patient = new PatientDto
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                BirthDate = DateTime.Parse("1990-01-01")
            },
            Medicaments = new List<MedicamentRequestDto>
            {
                new MedicamentRequestDto
                {
                    IdMedicament = 999,
                    Dose = 1,
                    Description = "1x dziennie"
                }
            }
        };

        var errorMessage = "Medykamenty o id 999 nie istnieją.";
        _mockService.Setup(s => s.CreatePrescriptionAsync(request))
            .ThrowsAsync(new ArgumentException(errorMessage));
        
        var result = await _controller.CreatePrescription(request);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
    
    [Fact]
        public async Task GetPatientDetails_For_ValidPatientId()
        {
            var patientId = 1;
            var expectedPatient = new PatientDetailsResponseDto
            {
                IdPatient = patientId,
                FirstName = "Jan",
                LastName = "Kowalski",
                BirthDate = DateTime.Parse("1990-01-01"),
                Prescriptions = new List<PrescriptionDto>
                {
                    new PrescriptionDto
                    {
                        IdPrescription = 1,
                        Date = DateTime.Now.AddDays(-7),
                        DueDate = DateTime.Now.AddDays(23),
                        Doctor = new DoctorDto
                        {
                            IdDoctor = 1,
                            FirstName = "Anna",
                            LastName = "Nowak",
                            Email = "anna.nowak@clinic.pl"
                        },
                        Medicaments = new List<MedicamentDto>
                        {
                            new MedicamentDto
                            {
                                IdMedicament = 1,
                                Name = "Paracetamol",
                                Description = "Lek przeciwbólowy",
                                Type = "Tabletka",
                                Dose = 2,
                                Details = "2x dziennie"
                            }
                        }
                    }
                }
            };

            _mockService.Setup(s => s.GetPatientDetailsAsync(patientId))
                         .ReturnsAsync(expectedPatient);
            
            var result = await _controller.GetPatientDetails(patientId);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var patient = Assert.IsType<PatientDetailsResponseDto>(okResult.Value);
            
            Assert.Equal(expectedPatient.IdPatient, patient.IdPatient);
            Assert.Equal(expectedPatient.FirstName, patient.FirstName);
            Assert.Equal(expectedPatient.LastName, patient.LastName);
            Assert.Equal(expectedPatient.BirthDate, patient.BirthDate);
            Assert.Single(patient.Prescriptions);
        }
        
    [Fact]
    public async Task GetPatientDetails_For_NonExistentPatient()
    {
        var patientId = 999;
        var errorMessage = $"Pacjent o id {patientId} nie istnieje!";
            
        _mockService.Setup(s => s.GetPatientDetailsAsync(patientId))
            .ThrowsAsync(new KeyNotFoundException(errorMessage));
        
        var result = await _controller.GetPatientDetails(patientId);
        
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(errorMessage, notFoundResult.Value);
    }
}
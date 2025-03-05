// UnitTests/AppointmentRepositoryUnitTests.cs
using Moq;
using Xunit;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PracticaJWTcore.Tests.MoqTest
{
    public class AppointmentRepositoryUnitTests
    {
        private readonly Mock<DbSet<Appointment>> _mockSet;
        private readonly Mock<PracticaJWTcoreContext> _mockContext;
        private readonly AppointmentRepository _repository;

        public AppointmentRepositoryUnitTests()
        {
            _mockSet = new Mock<DbSet<Appointment>>();
            _mockContext = new Mock<PracticaJWTcoreContext>();

            // Configuración básica del mock
            _mockContext.Setup(c => c.Appointments).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);

            _repository = new AppointmentRepository(_mockContext.Object);
        }

        [Fact]
        public async Task CreateAppointment_ShouldCallAddAndSaveChanges()
        {
            // Arrange
            var testAppointment = new Appointment
            {
                AppointmentDate = DateTime.Now,
                Comments = "Hola",
                EmployeeId = 1,
                VehicleId = 1
            };

            // Configurar el mock para simular la generación del ID
            long expectedId = 1;
            _mockSet.Setup(m => m.Add(It.IsAny<Appointment>()))
                .Callback<Appointment>(a => a.AppointmentId = expectedId);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);

            // Act
            var result = await _repository.CreateAppointment(testAppointment);

            // Assert
            _mockSet.Verify(m => m.Add(It.IsAny<Appointment>()), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.True(result>0); // Verifica el ID específico
        }

        [Fact]
        public async Task DeleteAppointment_ShouldRemoveEntity()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1 };
       var deleted=  _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(appointment);

            // Act
            var result = await _repository.DeleteApointment(1);

            // Assert
            _mockSet.Verify(m => m.Remove(It.IsAny<Appointment>()), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.True(result);
            Assert.Null(deleted);
        }
    }
}
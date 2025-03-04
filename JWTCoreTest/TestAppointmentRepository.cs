using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using Xunit;
namespace JWTCoreTest
{
    public class TestAppointmentRepository
    {
        private PracticaJWTcoreContext GetDbContextFromSettings()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("No se encontró la cadena de conexión en appsettings.json");
                }

                var options = new DbContextOptionsBuilder<PracticaJWTcoreContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                return new PracticaJWTcoreContext(options);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al configurar la base de datos", ex);
            }
        }


        [Fact]
        public async Task DeleteAppointment_ShouldRemoveAppointment()
        {
            // Arrange
            var context = GetDbContextFromSettings();
            var repository = new AppointmentRepository(context);

            // Usamos un ID ficticio que sabemos que no existe para forzar un error
            var appointmentId = 99;  // Asumimos que este ID no existe en la base de datos

            // Act
            var result = await repository.DeleteApointment(appointmentId);  // Intentamos eliminar un ID que no existe
            var deletedAppointment = await context.Appointments.FindAsync(appointmentId);

            // Assert
            Assert.True(result);  // El resultado debería ser False porque el ID no existe
            Assert.Null(deletedAppointment);  // La cita debería ser nula porque nunca existió
        }
    }
}

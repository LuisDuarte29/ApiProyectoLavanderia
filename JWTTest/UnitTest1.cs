using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PracticaJWTcore.Models;
using PracticaJWTcore.Repositorios;
using PracticaJWTcore.Services;
using Xunit;

namespace JWTTest
{
    public class UnitTest1
    {
        private readonly IAppointmentServices _appointmentServices;
        private readonly PracticaJWTcoreContext _appointmentRepos;
        public UnitTest1(IAppointmentServices appointmentServices, PracticaJWTcoreContext appointmentRepos)
        {
            _appointmentServices = appointmentServices;
            _appointmentRepos = appointmentRepos;
        }


        [Fact]
        public async Task DeleteAppointment_ShouldRemoveAppointment()
        {
            // Usamos un ID ficticio que sabemos que no existe para forzar un error
            var appointmentId = 99;  // Asumimos que este ID no existe en la base de datos

            // Act
            var result = await _appointmentServices.DeleteApointment(appointmentId);  // Intentamos eliminar un ID que no existe
            var deletedAppointment = await _appointmentRepos.Appointments.FindAsync(appointmentId);

            // Assert
            Assert.True(result);  // El resultado debería ser False porque el ID no existe
            Assert.Null(deletedAppointment);  // La cita debería ser nula porque nunca existió
        }
        [Fact]
        public async Task GetAppointment()
        {
            long id = 5;
            var result=await _appointmentRepos.Appointments.FirstAsync(x => x.AppointmentId == id);
            Assert.NotNull(result);
            Assert.IsType<Appointment>(result);
            Assert.Single((System.Collections.IEnumerable)result);
        }
        [Fact]
        public async Task GetAppointmentAll()
        {
            var result= await _appointmentRepos.Appointments.ToListAsync();
            Assert.NotNull(result);
            Assert.IsType<Appointment>(result);
        }
    }
}
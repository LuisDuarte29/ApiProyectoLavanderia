using ApiSwagger.Dtos;
using PracticaJWTcore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PracticaJWTcore.Repositorios
{
    public class CustomerDataBaseCustomer: DbContext
    {

        public CustomerDataBaseCustomer(DbContextOptions<CustomerDataBaseCustomer> options) : base(options)
        {

        }
        public DbSet<CustomerEntity> Customer { get; set; }

       

    }
}

using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=FiapTechChallenge;User Id=sa;Password=Your_password123;TrustServerCertificate=True");
            //optionsBuilder.UseSqlServer("Server=LAPTOP-OUPT3G77\\SQLEXPRESS;Database=FiapTechChallenge;Trusted_Connection=True;TrustServerCertificate=True");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

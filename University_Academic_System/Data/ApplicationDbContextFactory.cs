using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;



namespace University_Academic_System.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<UniversityDbContext>
    {
        public UniversityDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Missing DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new UniversityDbContext(optionsBuilder.Options);
        }
    }
}

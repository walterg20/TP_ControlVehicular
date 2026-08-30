using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TP_ControlVehicular.Datos.Data.Migracion
{
    public class CVDbContextFactory : IDesignTimeDbContextFactory<CVDbContext>
    {
        public CVDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection")
                ?? Environment.GetEnvironmentVariable("DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=TP_ControlVehicular;Trusted_Connection=True;";

            var optionsBuilder = new DbContextOptionsBuilder<CVDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new CVDbContext(optionsBuilder.Options);
        }
    }
}

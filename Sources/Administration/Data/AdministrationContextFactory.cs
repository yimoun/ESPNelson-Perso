using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Administration.Data.Context;
using Pomelo.EntityFrameworkCore.MySql;

namespace Administration.Data
{
    public class AdministrationContextFactory : IDesignTimeDbContextFactory<AdministrationContext>
    {
        public AdministrationContext CreateDbContext(string[] args)
        {
            var configuration = ConfigurationService.GetConfiguration();

            // Récupérer la chaîne de connexion à partir de appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AdministrationContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new AdministrationContext(optionsBuilder.Options);
        }
    }
}

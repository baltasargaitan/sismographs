using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infraestructura.Persistencia
{
    // Esta clase solo se usa en tiempo de diseño (para 'dotnet ef')
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING_DEFAULTCONNECTION")
                ?? "Server=localhost\\SQLEXPRESS;Database=SistemaSismografosDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

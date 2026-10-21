using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ECommerce.Infrastructure.Context;

public class UygulamaDbContextFactory : IDesignTimeDbContextFactory<UygulamaDbContext>
{
    public UygulamaDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var apiDirectory = new[]
        {
            Path.Combine(currentDirectory, "ECommerce.WebAPI"),
            Path.GetFullPath(Path.Combine(currentDirectory, "..", "ECommerce.WebAPI"))
        }.FirstOrDefault(Directory.Exists) ?? currentDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
            .AddUserSecrets(typeof(UygulamaDbContextFactory).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<UygulamaDbContext>();

        // appsettings.json dosyanızdaki anahtar adı "VarsayilanBaglanti" olduğu için burayı güncelledik
        var connectionString = configuration.GetConnectionString("VarsayilanBaglanti");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("PostgreSQL connection string is missing. Configure User Secrets or ConnectionStrings__VarsayilanBaglanti.");
        }

        connectionString = PostgreSqlConnectionStringNormalizer.Normalize(connectionString);

        builder.UseNpgsql(connectionString);

        return new UygulamaDbContext(builder.Options);
    }
}

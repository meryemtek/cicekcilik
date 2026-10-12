using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ECommerce.Infrastructure.Context;

public class UygulamaDbContextFactory : IDesignTimeDbContextFactory<UygulamaDbContext>
{
    public UygulamaDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ECommerce.WebAPI"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var builder = new DbContextOptionsBuilder<UygulamaDbContext>();

        // appsettings.json dosyanızdaki anahtar adı "VarsayilanBaglanti" olduğu için burayı güncelledik
        var connectionString = configuration.GetConnectionString("VarsayilanBaglanti");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Bağlantı dizesi (ConnectionStrings:VarsayilanBaglanti) appsettings.json içinde bulunamadı.");
        }

        builder.UseSqlServer(connectionString);

        return new UygulamaDbContext(builder.Options);
    }
}
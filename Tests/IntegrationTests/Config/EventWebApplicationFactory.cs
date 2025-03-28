using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EM.DAL.EF;
using Microsoft.AspNetCore.Hosting;

namespace Tests.IntegrationTests.Config;

public class EventWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public SqliteConnection SqliteInMemoryConnection { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EmDbContext>));
            if (dbContextOptionsDescriptor != null)
            {
                services.Remove(dbContextOptionsDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            services.AddSingleton<DbConnection>(container =>
            {
                SqliteInMemoryConnection = new SqliteConnection("DataSource=:memory:");
                SqliteInMemoryConnection.Open();
                return SqliteInMemoryConnection;
            });

            services.AddDbContext<EmDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            // Maak de database aan voordat de host start
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EmDbContext>();
        });

        builder.UseEnvironment("Test");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SqliteInMemoryConnection?.Close();
            SqliteInMemoryConnection?.Dispose();
        }
        base.Dispose(disposing);
    }
}
using System.Data.Common;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EM.DAL.EF;

namespace Tests.IntegrationTests.Config;

public class ExtendedWebApplicationFactoryWithMockAuth<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    // Open gehouden in-memory SQLite connectie zodat de DB leeft zolang de factory leeft.
    public SqliteConnection SqliteInMemoryConnection { get; private set; } = default!;

    // Dynamisch aanpasbare claim-seed (via SetAuthenticatedUser)
    private MockClaimSeed _mockClaimSeed = new MockClaimSeed(Array.Empty<Claim>());

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Verwijder bestaande EmDbContext-registraties
            var dbContextOptionsDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EmDbContext>));
            if (dbContextOptionsDescriptor != null) services.Remove(dbContextOptionsDescriptor);

            var dbConnectionDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);

            // Één gedeelde open SQLite in-memory connectie
            services.AddSingleton<DbConnection>(_ =>
            {
                SqliteInMemoryConnection = new SqliteConnection("DataSource=:memory:");
                SqliteInMemoryConnection.Open(); // uiterst belangrijk
                return SqliteInMemoryConnection;
            });

            services.AddDbContext<EmDbContext>((sp, options) =>
            {
                var connection = sp.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            // Bouw een tijdelijke provider om de DB te initialiseren
            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EmDbContext>();
            db.Database.EnsureCreated(); // ← Maak tabellen effectief aan (geen SELECT-hack)
        });

        builder.ConfigureServices(services =>
        {
            // Maak "TestAuth" de default, laat Identity.Application met rust
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestAuth";
                    options.DefaultChallengeScheme    = "TestAuth";
                    options.DefaultScheme             = "TestAuth";
                })
                .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>(
                    "TestAuth", _ => { });

            // jouw claims-seed die de handler injecteert
            services.AddScoped(_ => _mockClaimSeed);
        });
    }

    /// <summary>
    /// Stel de "ingelogde" gebruiker in. Laat leeg voor anonieme flow.
    /// </summary>
    public ExtendedWebApplicationFactoryWithMockAuth<TProgram> SetAuthenticatedUser(params Claim[] claims)
    {
        _mockClaimSeed = new MockClaimSeed(claims ?? Array.Empty<Claim>());
        return this;
    }

    /// <summary>
    /// (Optioneel) Reset de testdatabase naar een lege staat.
    /// Handig om test-data leakage te voorkomen tussen tests.
    /// </summary>
    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EmDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
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

    // ----------------- Mock Auth implementatie -----------------

    public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly MockClaimSeed _claimSeed;

        public MockAuthenticationHandler(
            MockClaimSeed claimSeed,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _claimSeed = claimSeed;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = _claimSeed.GetSeeds();

            // Geen claims => anonieme gebruiker (NoResult), niét Fail()
            if (!claims.Any())
                return Task.FromResult(AuthenticateResult.NoResult());

            var identity  = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var ticket    = new AuthenticationTicket(principal, "TestAuth");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }


    public sealed class MockClaimSeed
    {
        private readonly IEnumerable<Claim> _seed;

        public MockClaimSeed(IEnumerable<Claim> seed) => _seed = seed ?? Enumerable.Empty<Claim>();
        public IEnumerable<Claim> GetSeeds() => _seed;
    }
}

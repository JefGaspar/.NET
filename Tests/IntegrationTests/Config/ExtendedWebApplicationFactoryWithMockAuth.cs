using System.Data.Common;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
    // (*) use of in-memory sqlite database
    public SqliteConnection SqliteInMemoryConnection { get; private set; }

    // (**) use of authenticated user
    private MockClaimSeed mockClaimSeed = new MockClaimSeed(new Claim[] { });

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // (*) use of in-memory sqlite database
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

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                SqliteInMemoryConnection = new SqliteConnection("DataSource=:memory:");
                var connection = SqliteInMemoryConnection;
                connection.Open();

                return connection;
            });

            services.AddDbContext<EmDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            // Maak de database aan voordat de host start
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EmDbContext>();

            // Voeg logging toe om te controleren of de database is aangemaakt
            Console.WriteLine("Database created: " + dbContext.Database.CanConnect());
            var eventsTableExists = dbContext.Database
                .ExecuteSqlRaw("SELECT name FROM sqlite_master WHERE type='table' AND name='Events';") > 0;
            Console.WriteLine("Events table exists: " + eventsTableExists);
        });

        // (**) use of authenticated user
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IAuthenticationSchemeProvider, MockSchemeProvider>();
            services.AddScoped<MockClaimSeed>(_ => mockClaimSeed);
        });

        builder.UseEnvironment("Test"); // Gebruik "Test"-omgeving om seeding over te slaan
    }

    public ExtendedWebApplicationFactoryWithMockAuth<TProgram> SetAuthenticatedUser(params Claim[] claimSeed)
    {
        mockClaimSeed = new MockClaimSeed(claimSeed);
        return this;
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

    public class MockSchemeProvider : AuthenticationSchemeProvider
    {
        public MockSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
        {
        }

        protected MockSchemeProvider(
            IOptions<AuthenticationOptions> options,
            IDictionary<string, AuthenticationScheme> schemes
        )
            : base(options, schemes)
        {
        }

        public override Task<AuthenticationScheme> GetSchemeAsync(string name)
        {
            return Task.FromResult(new AuthenticationScheme(
                IdentityConstants.ApplicationScheme,  // ← let op deze exacte string
                IdentityConstants.ApplicationScheme,
                typeof(MockAuthenticationHandler)
            ));
        }

    }

    public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly MockClaimSeed _claimSeed;

        public MockAuthenticationHandler(
            MockClaimSeed claimSeed,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            TimeProvider timeProvider) // Gebruik TimeProvider
            : base(options, logger, encoder)
        {
            _claimSeed = claimSeed;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!_claimSeed.GetSeeds().Any())
                return Task.FromResult(AuthenticateResult.Fail("No authenticated user seeded for test!"));

            var claimsIdentity = new ClaimsIdentity(_claimSeed.GetSeeds(), IdentityConstants.ApplicationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var ticket = new AuthenticationTicket(claimsPrincipal, IdentityConstants.ApplicationScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class MockClaimSeed
    {
        private readonly IEnumerable<Claim> _seed;

        public MockClaimSeed(IEnumerable<Claim> seed)
        {
            _seed = seed;
        }

        public IEnumerable<Claim> GetSeeds() => _seed;
    }
    
    
}
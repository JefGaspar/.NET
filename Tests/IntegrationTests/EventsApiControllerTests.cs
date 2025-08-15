using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using EM.BL;
using EM.BL.Domain;
using EM.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Tests.IntegrationTests.Config;

namespace Tests.IntegrationTests
{
    public class EventsApiControllerTests : IClassFixture<ExtendedWebApplicationFactoryWithMockAuth<Program>>
    {
        private readonly ExtendedWebApplicationFactoryWithMockAuth<Program> _factory;

        public EventsApiControllerTests(ExtendedWebApplicationFactoryWithMockAuth<Program> factory)
        {
            _factory = factory;
            _factory.ResetDatabase(); // schone lei per testclass
        }

        // Klein type om API-respons netjes te parsen
        private sealed record UpdateResponse(bool success, decimal newPrice);

        [Fact]
        public async Task UpdateEvent_ShouldReturnUnauthorized_WhenAnonymous()
        {
            // Arrange (geen claims)
            using var scope = _factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IManager>();
            var ev = manager.AddEvent("Dance", System.DateTime.Today.AddDays(5), 10m, "desc", EventCategory.Festival, "owner-1");

            var client = _factory.CreateClient();
            var update = new { ticketPrice = 15.00m };

            // Act
            var response = await client.PutAsJsonAsync($"/api/events/{ev.EventId}", update);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateEvent_ShouldReturnNotFound_WhenIdUnknown()
        {
            // Arrange (gewoon geauth. user, id bestaat niet)
            var client = _factory
                .SetAuthenticatedUser(
                    new Claim(ClaimTypes.Name, "User"),
                    new Claim(ClaimTypes.NameIdentifier, "user123"))
                .CreateClient();

            var update = new { ticketPrice = 40.00m };

            // Act
            var response = await client.PutAsJsonAsync("/api/events/9999", update);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateEvent_ShouldReturnBadRequest_WhenPayloadInvalid()
        {
            // Arrange (auth user, lege payload → 400 vóór ownercheck)
            using var scope = _factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IManager>();
            var ev = manager.AddEvent("Invalid", System.DateTime.Today.AddDays(7), 12m, "desc", EventCategory.Festival, "user123");

            var client = _factory
                .SetAuthenticatedUser(
                    new Claim(ClaimTypes.Name, "User"),
                    new Claim(ClaimTypes.NameIdentifier, "user123"))
                .CreateClient();

            // Act
            var response = await client.PutAsJsonAsync($"/api/events/{ev.EventId}", new { /* empty */ });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ------------------------- Extra scenario’s (sterk aangeraden) -------------------------
        // Vereisen dat de Identity-user(s) echt bestaan (UserManager) en, voor admin, ook de rol.

        [Fact]
        public async Task UpdateEvent_ShouldReturnOkAndUpdatePrice_WhenOwner()
        {
            // Arrange
            const string ownerId = "owner-123";
            await EnsureUserExistsAsync(ownerId, "owner@test.local"); // seed Identity-user

            using (var scope = _factory.Services.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<IManager>();
                manager.AddEvent("Tech Expo", System.DateTime.Today.AddDays(5), 25.00m, "A tech expo", EventCategory.Conference, ownerId);
            }

            var client = _factory
                .SetAuthenticatedUser(
                    new Claim(ClaimTypes.NameIdentifier, ownerId),
                    new Claim(ClaimTypes.Name, "owner@test.local"))
                .CreateClient();

            var update = new { ticketPrice = 55.00m };

            // Act
            // Zoek het echte Id (eerste en enige event)
            int eventId;
            using (var verify = _factory.Services.CreateScope())
            {
                var db = verify.ServiceProvider.GetRequiredService<EmDbContext>();
                eventId = await db.Events.Select(e => e.EventId).FirstAsync();
            }

            var response = await client.PutAsJsonAsync($"/api/events/{eventId}", update);

            // Assert
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<UpdateResponse>();
            Assert.NotNull(body);
            Assert.True(body!.success);
            Assert.Equal(55.00m, body.newPrice);

            // DB-check
            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<EmDbContext>();
            var updated = await verifyDb.Events.FindAsync(eventId);
            Assert.Equal(55.00m, updated!.TicketPrice);
        }

        [Fact]
        public async Task UpdateEvent_ShouldReturnForbidden_WhenUserIsNotOwnerAndNotAdmin()
        {
            // Arrange
            const string ownerId = "owner-1";
            const string otherId = "user-2";
            await EnsureUserExistsAsync(ownerId, "owner@test.local");
            await EnsureUserExistsAsync(otherId, "other@test.local");

            int eventId;
            using (var scope = _factory.Services.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<IManager>();
                var ev = manager.AddEvent("Party", System.DateTime.Today.AddDays(15), 20m, "desc", EventCategory.Festival, ownerId);
                eventId = ev.EventId;
            }

            var client = _factory
                .SetAuthenticatedUser(
                    new Claim(ClaimTypes.NameIdentifier, otherId),
                    new Claim(ClaimTypes.Name, "other@test.local"))
                .CreateClient();

            var update = new { ticketPrice = 25.00m };

            // Act
            var response = await client.PutAsJsonAsync($"/api/events/{eventId}", update);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateEvent_ShouldReturnOk_WhenAdminEditsForeignEvent()
        {
            // Arrange
            const string ownerId = "owner-9";
            const string adminId = "admin-9";
            await EnsureUserExistsAsync(ownerId, "o@test.local");
            await EnsureUserExistsAsync(adminId, "a@test.local");
            await EnsureRoleExistsAndAssignAsync(adminId, "Admin");

            int eventId;
            using (var scope = _factory.Services.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<IManager>();
                eventId = manager.AddEvent("Conf", System.DateTime.Today.AddDays(10), 30m, "conf", EventCategory.Conference, ownerId).EventId;
            }

            var client = _factory
                .SetAuthenticatedUser(
                    new Claim(ClaimTypes.NameIdentifier, adminId),
                    new Claim(ClaimTypes.Name, "a@test.local"),
                    new Claim(ClaimTypes.Role, "Admin"))
                .CreateClient();

            var update = new { ticketPrice = 60.00m };

            // Act
            var response = await client.PutAsJsonAsync($"/api/events/{eventId}", update);

            // Assert
            response.EnsureSuccessStatusCode();
            var dto = await response.Content.ReadFromJsonAsync<UpdateResponse>();
            Assert.True(dto!.success);
            Assert.Equal(60.00m, dto.newPrice);
        }

        // ---------- helpers ----------

        private async Task EnsureUserExistsAsync(string id, string email)
        {
            using var scope = _factory.Services.CreateScope();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var existing = await users.FindByIdAsync(id);
            if (existing != null) return;

            var user = new IdentityUser
            {
                Id = id,
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true
            };
            var result = await users.CreateAsync(user); // geen password nodig voor tests
            Assert.True(result.Succeeded, "Failed to create test user");
        }

        private async Task EnsureRoleExistsAndAssignAsync(string userId, string roleName)
        {
            using var scope = _factory.Services.CreateScope();
            var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (!await roles.RoleExistsAsync(roleName))
            {
                var r = await roles.CreateAsync(new IdentityRole(roleName));
                Assert.True(r.Succeeded, "Failed to create test role");
            }

            var user = await users.FindByIdAsync(userId);
            Assert.NotNull(user);
            if (!await users.IsInRoleAsync(user!, roleName))
            {
                var add = await users.AddToRoleAsync(user!, roleName);
                Assert.True(add.Succeeded, "Failed to add user to role");
            }
        }
    }
}

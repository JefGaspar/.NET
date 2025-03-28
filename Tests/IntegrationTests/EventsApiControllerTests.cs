using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using EM.BL;
using EM.BL.Domain;
using EM.DAL.EF;
using Microsoft.AspNetCore.Identity;
using Tests.IntegrationTests.Config;

namespace Tests.IntegrationTests;

public class EventsApiControllerTests : IClassFixture<ExtendedWebApplicationFactoryWithMockAuth<Program>>
{
    private readonly ExtendedWebApplicationFactoryWithMockAuth<Program> _factory;

    public EventsApiControllerTests(ExtendedWebApplicationFactoryWithMockAuth<Program> factory)
    {
        _factory = factory;
    }
    /*
    [Fact]
    public async Task UpdateEvent_AsOwner_ReturnsOkAndUpdatesPrice()
    {
        // Arrange
        var userId = "db101bb8-0c85-4936-b859-153984448880"; // Id van user2@example.com
        var factoryWithAuth = _factory.SetAuthenticatedUser(
            new Claim(ClaimTypes.Name, "user2@example.com"),
            new Claim(ClaimTypes.NameIdentifier, userId)
        );

        using var scope = factoryWithAuth.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmDbContext>();

        // Zorg ervoor dat de database is geïnitialiseerd
        dbContext.Database.EnsureCreated();

        var @event = manager.AddEvent("Concert", DateTime.Today.AddDays(10), 30m, "desc", EventCategory.Music, userId);

        var client = factoryWithAuth.CreateClient();
        var update = new { ticketPrice = 45.00m };

        // Act
        var response = await client.PutAsJsonAsync($"/api/Events/{@event.EventId}", update);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.Equal(45.00m, (decimal)content!.newPrice);

        // Controleer of de prijs in de database is bijgewerkt
        var updatedEvent = await dbContext.Events.FindAsync(@event.EventId);
        Assert.Equal(45.00m, updatedEvent.TicketPrice);
    }
    [Fact]
    public async Task UpdateEvent_AsAdmin_ReturnsOk()
    {
        // Arrange
        var adminUserId = "a3f63bff-151c-48eb-ae8d-2593747599d"; // Id van user1@example.com (Admin)
        var factoryWithAuth = _factory.SetAuthenticatedUser(
            new Claim(ClaimTypes.Name, "user1@example.com"),
            new Claim(ClaimTypes.NameIdentifier, adminUserId),
            new Claim(ClaimTypes.Role, "Admin")
        );

        using var scope = factoryWithAuth.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmDbContext>();

        // Zorg ervoor dat de database is geïnitialiseerd
        dbContext.Database.EnsureCreated();

        // Maak een event aan dat eigendom is van een andere gebruiker
        var otherUserId = "db101bb8-0c85-4936-b859-153984448880"; // Id van user2@example.com
        var @event = manager.AddEvent("Tech Expo", DateTime.Today.AddDays(5), 25.00m, "A tech expo", EventCategory.Conference, otherUserId);

        var client = factoryWithAuth.CreateClient();
        var update = new { ticketPrice = 60.00m };

        // Act
        var response = await client.PutAsJsonAsync($"/api/Events/{@event.EventId}", update);

        // Assert
        response.EnsureSuccessStatusCode();

        // Controleer of de prijs in de database is bijgewerkt
        var updatedEvent = await dbContext.Events.FindAsync(@event.EventId);
        Assert.Equal(60.00m, updatedEvent.TicketPrice);
    }
    [Fact]
    public async Task UpdateEvent_AsUnauthorizedUser_ReturnsForbidden()
    {
        // Arrange
        var ownerId = "db101bb8-0c85-4936-b859-153984448880"; // Id van user2@example.com
        var otherUserId = "f802b41f-9792-4f3a-9717-c83dce30b790"; // Id van user3@example.com
        var factoryWithAuth = _factory.SetAuthenticatedUser(
            new Claim(ClaimTypes.Name, "user3@example.com"),
            new Claim(ClaimTypes.NameIdentifier, otherUserId)
        );

        using var scope = factoryWithAuth.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmDbContext>();

        // Zorg ervoor dat de database is geïnitialiseerd
        dbContext.Database.EnsureCreated();

        var @event = manager.AddEvent("Party", DateTime.Today.AddDays(15), 20m, "desc", EventCategory.Festival, ownerId);

        var client = factoryWithAuth.CreateClient();
        var update = new { ticketPrice = 25.00m };

        // Act
        var response = await client.PutAsJsonAsync($"/api/Events/{@event.EventId}", update);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }*/

[Fact]
public async Task UpdateEvent_AsAnonymous_ReturnsUnauthorized()
{
    // Arrange
    var factory = _factory; // geen authenticated user

    using var scope = factory.Services.CreateScope();
    var manager = scope.ServiceProvider.GetRequiredService<IManager>();
    var @event = manager.AddEvent("Dance", DateTime.Today.AddDays(5), 10m, "desc", EventCategory.Festival, "someone");

    var client = factory.CreateClient();
    var update = new { ticketPrice = 15.00m };

    // Act
    var response = await client.PutAsJsonAsync($"/api/events/{@event.EventId}", update);

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}

[Fact]
public async Task UpdateEvent_EventDoesNotExist_ReturnsNotFound()
{
    // Arrange
    var factoryWithAuth = _factory.SetAuthenticatedUser(
        new Claim(ClaimTypes.Name, "User"),
        new Claim(ClaimTypes.NameIdentifier, "user123")
    );


    var client = factoryWithAuth.CreateClient();
    var update = new { ticketPrice = 40.00m };

    // Act
    var response = await client.PutAsJsonAsync("/api/events/9999", update);

    // Assert
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}

[Fact]
public async Task UpdateEvent_WithInvalidPayload_ReturnsBadRequest()
{
    // Arrange
    var factoryWithAuth = _factory.SetAuthenticatedUser(
        new Claim(ClaimTypes.Name, "User"),
        new Claim(ClaimTypes.NameIdentifier, "user123")
    );


    using var scope = factoryWithAuth.Services.CreateScope();
    var manager = scope.ServiceProvider.GetRequiredService<IManager>();
    var @event = manager.AddEvent("Invalid", DateTime.Today.AddDays(7), 12m, "desc", EventCategory.Festival, "user123");

    var client = factoryWithAuth.CreateClient();

    // Act
    var response = await client.PutAsJsonAsync($"/api/events/{@event.EventId}", new { }); // Lege payload

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}


}
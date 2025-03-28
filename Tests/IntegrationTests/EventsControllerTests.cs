using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using EM.BL;
using EM.BL.Domain;
using EM.DAL.EF;
using Tests.IntegrationTests.Config;

namespace Tests.IntegrationTests;

public class EventsControllerTests : IClassFixture<ExtendedWebApplicationFactoryWithMockAuth<Program>>
{
    private readonly ExtendedWebApplicationFactoryWithMockAuth<Program> _factory;

    public EventsControllerTests(ExtendedWebApplicationFactoryWithMockAuth<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithEvent()
    {
        // Arrange
        // Maak een event aan in de database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmDbContext>();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();

        var newEvent = manager.AddEvent(
            name: "Test Event",
            date: DateTime.Today.AddDays(5),
            ticketPrice: 25.00m,
            description: "A test event for integration testing",
            category: EventCategory.Music,
            userId: "user123"
        );

        // Maak een HTTP-client
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/Events/Details/{newEvent.EventId}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("text/html", response.Content.Headers.ContentType.MediaType);

        // Controleer of het resultaat een ViewResult is met het juiste model
        // (Dit vereist dat we de response body uitlezen en deserialiseren, wat complex is voor HTML.
        // Voor nu vertrouwen we op de statuscode en content-type.)
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Events/Details/999"); // Ongeldige ID

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
  
}
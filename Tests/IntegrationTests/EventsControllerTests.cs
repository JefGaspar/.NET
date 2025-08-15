using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using EM.BL;
using EM.BL.Domain;
using EM.DAL.EF;
using Tests.IntegrationTests.Config;

namespace Tests.IntegrationTests
{
    public class EventControllerIntegrationTests : IClassFixture<ExtendedWebApplicationFactoryWithMockAuth<Program>>
    {
        private readonly ExtendedWebApplicationFactoryWithMockAuth<Program> _factory;

        public EventControllerIntegrationTests(ExtendedWebApplicationFactoryWithMockAuth<Program> factory)
        {
            _factory = factory;
            // (optioneel) zorg per testclass voor een schone DB
            _factory.ResetDatabase();
        }

        [Fact]
        public async Task Details_ShouldReturnViewWithEvent_WhenIdValid()
        {
            // Arrange
            int eventId;

            using (var seedScope = _factory.Services.CreateScope())
            {
                var manager = seedScope.ServiceProvider.GetRequiredService<IManager>();

                var created = manager.AddEvent(
                    name: "Test Event",
                    date: System.DateTime.Today.AddDays(5),
                    ticketPrice: 25.00m,
                    description: "A test event for integration testing",
                    category: EventCategory.Music,
                    userId: "user123"
                );

                eventId = created.EventId; // sla alleen ID op
            }

// eventueel: extra verify-scope om te checken of event in DB staat
            using (var verifyScope = _factory.Services.CreateScope())
            {
                var verifyCtx = verifyScope.ServiceProvider.GetRequiredService<EmDbContext>();
                var fromDb = await verifyCtx.Events.FindAsync(eventId);
                Assert.NotNull(fromDb);
            }

            var client = _factory.CreateClient();

// Act
            var response = await client.GetAsync($"/Event/Details/{eventId}");


            // Assert
            response.EnsureSuccessStatusCode();                       // 200 OK
            Assert.Equal("text/html", response.Content.Headers.ContentType!.MediaType);

            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Event", html);                      // inhoud bevat naam
        }

        [Fact]
        public async Task Details_ShouldReturnNotFound_WhenIdUnknown()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Event/Details/999"); // verkeerde ID, maar juiste controller-route

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

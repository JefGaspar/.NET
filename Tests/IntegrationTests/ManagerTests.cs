using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

using EM.BL;
using EM.BL.Domain;
using EM.DAL.EF;
using Tests.IntegrationTests.Config;

namespace Tests.IntegrationTests
{
    public class ManagerTests : IClassFixture<ExtendedWebApplicationFactoryWithMockAuth<Program>>
    {
        private readonly ExtendedWebApplicationFactoryWithMockAuth<Program> _factory;

        public ManagerTests(ExtendedWebApplicationFactoryWithMockAuth<Program> factory)
        {
            _factory = factory;
            _factory.ResetDatabase();

        }

        // ---------- AddEvent ----------

        [Fact]
        public void AddEvent_ShouldPersistAndReturnEvent_WhenInputIsValid()
        {
            // Arrange
            using var beforeScope = _factory.Services.CreateScope();
            var beforeCtx = beforeScope.ServiceProvider.GetRequiredService<EmDbContext>();
            int beforeCount = beforeCtx.Events.Count();

            using var scope = _factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IManager>();

            // Act
            var created = manager.AddEvent(
                name: "Concert",
                date: DateTime.Today.AddDays(10),
                ticketPrice: 50.00m,
                description: "A great concert event",
                category: EventCategory.Music,
                userId: "user123"
            );

            // Assert (nieuwe scope om EF-tracking te vermijden)
            using var verifyScope = _factory.Services.CreateScope();
            var verifyCtx = verifyScope.ServiceProvider.GetRequiredService<EmDbContext>();
            var fromDb = verifyCtx.Events.Find(created.EventId);

            Assert.NotNull(created);
            Assert.NotNull(fromDb);
            Assert.Equal(beforeCount + 1, verifyCtx.Events.Count());
            Assert.Equal("Concert", fromDb!.EventName);
            Assert.Equal(50.00m, fromDb.TicketPrice);
            Assert.Equal(EventCategory.Music, fromDb.Category);
            Assert.Equal("user123", fromDb.UserId);
        }

        [Fact]
        public void AddEvent_ShouldThrowValidationException_WhenNameMissing()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IManager>();

            // Act
            var ex = Assert.Throws<ValidationException>(() =>
                manager.AddEvent(
                    name: "",
                    date: DateTime.Today.AddDays(10),
                    ticketPrice: 50.00m,
                    description: "desc",
                    category: EventCategory.Music,
                    userId: "user123"
                )
            );

            // Assert (robuuster dan exact hele zin)
            Assert.Contains("EventName", ex.Message);
        }

        // ---------- ChangeEvent ----------

        [Fact]
        public void ChangeEvent_ShouldUpdateFields_WhenInputIsValid()
        {
            // Arrange
            using var arrangeScope = _factory.Services.CreateScope();
            var manager = arrangeScope.ServiceProvider.GetRequiredService<IManager>();

            var ev = manager.AddEvent(
                name: "Festival",
                date: DateTime.Today.AddDays(20),
                ticketPrice: 75.00m,
                description: "A fun festival",
                category: EventCategory.Festival,
                userId: "user456"
            );

            ev.TicketPrice = 100.00m;
            ev.EventDescription = "An updated festival description";

            // Act
            manager.ChangeEvent(ev);

            // Assert (nieuwe scope)
            using var verifyScope = _factory.Services.CreateScope();
            var verifyCtx = verifyScope.ServiceProvider.GetRequiredService<EmDbContext>();
            var updated = verifyCtx.Events.Find(ev.EventId);

            Assert.NotNull(updated);
            Assert.Equal(100.00m, updated!.TicketPrice);
            Assert.Equal("An updated festival description", updated.EventDescription);
        }

        [Fact]
        public void ChangeEvent_ShouldThrowValidationException_WhenNameMissing()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IManager>();

            var ev = manager.AddEvent(
                name: "Festival",
                date: DateTime.Today.AddDays(20),
                ticketPrice: 75.00m,
                description: "A fun festival",
                category: EventCategory.Festival,
                userId: "user456"
            );

            ev.EventName = "";

            // Act
            var ex = Assert.Throws<ValidationException>(() => manager.ChangeEvent(ev));

            // Assert
            Assert.Contains("EventName", ex.Message);
        }

        [Fact]
        public void ChangeEvent_ShouldThrowDbUpdateConcurrency_WhenIdUnknown()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IManager>();
            var ghost = new Event {
                EventId = 999, EventName = "Ghost", EventDate = DateTime.Today.AddDays(1),
                TicketPrice = 10m, EventDescription = "nope", Category = EventCategory.Music, UserId = "u"
            };

            // Act + Assert
            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>(() => manager.ChangeEvent(ghost));
        }

    }

    /// <summary>
    /// Aparte testklasse zodat het sluiten van de DB-verbinding
    /// geen impact heeft op de gedeelde fixture van de andere tests.
    /// </summary>
    public class ManagerDbFailureTests
    {
        [Fact]
        public void AddEvent_ShouldThrowDbUpdateException_WhenDatabaseIsUnavailable()
        {
            // Arrange: gebruik een geïsoleerde factory i.p.v. de class fixture
            using var isolatedFactory = new ExtendedWebApplicationFactoryWithMockAuth<Program>();
            using var scope = isolatedFactory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IManager>();

            // Simuleer DB failure enkel in deze test
            isolatedFactory.SqliteInMemoryConnection.Close();

            // Act + Assert
            Assert.Throws<DbUpdateException>(() =>
                manager.AddEvent(
                    name: "Concert",
                    date: DateTime.Today.AddDays(10),
                    ticketPrice: 50.00m,
                    description: "desc",
                    category: EventCategory.Music,
                    userId: "user123"
                )
            );
        }
    }
}

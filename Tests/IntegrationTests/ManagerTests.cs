using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using EM.BL;
using EM.BL.Domain;
using EM.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Tests.IntegrationTests.Config;

namespace Tests.IntegrationTests;

public class ManagerTests : IClassFixture<EventWebApplicationFactory<Program>>
{
    private readonly EventWebApplicationFactory<Program> _factory;

    public ManagerTests(EventWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region AddEvent Tests
/*
    [Fact]
    public void AddEvent_GivenValidData_SavesEventToDbAndReturnsNewEvent()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();
        var context = scope.ServiceProvider.GetRequiredService<EmDbContext>();
        context.Database.EnsureCreated();

        string name = "Concert";
        DateTime date = DateTime.Today.AddDays(10);
        decimal? ticketPrice = 50.00m;
        string description = "A great concert event";
        EventCategory category = EventCategory.Music;
        string userId = "user123";

        var createdEvent = manager.AddEvent(name, date, ticketPrice, description, category, userId);

        Assert.NotNull(createdEvent);
        Assert.Equal(name, createdEvent.EventName);
        Assert.Equal(date, createdEvent.EventDate);
        Assert.Equal(ticketPrice, createdEvent.TicketPrice);
        Assert.Equal(description, createdEvent.EventDescription);
        Assert.Equal(category, createdEvent.Category);
        Assert.Equal(userId, createdEvent.UserId);
        
        Assert.NotNull(context.Events.Find(createdEvent.EventId));
    }
*/
    [Fact]
    public void AddEvent_GivenInvalidData_ThrowsValidationException()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();

        string name = "";
        DateTime date = DateTime.Today.AddDays(10);
        decimal? ticketPrice = 50.00m;
        string description = "A great concert event";
        EventCategory category = EventCategory.Music;
        string userId = "user123";

        var exception = Assert.Throws<ValidationException>(() =>
            manager.AddEvent(name, date, ticketPrice, description, category, userId));
        Assert.Contains("The EventName field is required", exception.Message);
    }

    [Fact]
    public void AddEvent_WhenDatabaseFails_ThrowsException()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();

        string name = "Concert";
        DateTime date = DateTime.Today.AddDays(10);
        decimal? ticketPrice = 50.00m;
        string description = "A great concert event";
        EventCategory category = EventCategory.Music;
        string userId = "user123";

        _factory.SqliteInMemoryConnection.Close();

        Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(() =>
            manager.AddEvent(name, date, ticketPrice, description, category, userId));
    }

    #endregion

    #region ChangeEvent Tests

    [Fact]
    public void ChangeEvent_GivenValidData_UpdatesEventInDb()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmDbContext>();

        var newEvent = manager.AddEvent(
            name: "Festival",
            date: DateTime.Today.AddDays(20),
            ticketPrice: 75.00m,
            description: "A fun festival",
            category: EventCategory.Festival,
            userId: "user456"
        );

        newEvent.TicketPrice = 100.00m;
        newEvent.EventDescription = "An updated festival description";

        manager.ChangeEvent(newEvent);

        var updatedEvent = dbContext.Events.Find(newEvent.EventId);
        Assert.NotNull(updatedEvent);
        Assert.Equal(100.00m, updatedEvent.TicketPrice);
        Assert.Equal("An updated festival description", updatedEvent.EventDescription);
    }

    [Fact]
    public void ChangeEvent_GivenInvalidData_ThrowsValidationException()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmDbContext>();

        var newEvent = manager.AddEvent(
            name: "Festival",
            date: DateTime.Today.AddDays(20),
            ticketPrice: 75.00m,
            description: "A fun festival",
            category: EventCategory.Festival,
            userId: "user456"
        );

        newEvent.EventName = "";

        var exception = Assert.Throws<ValidationException>(() => manager.ChangeEvent(newEvent));
        Assert.Contains("The EventName field is required", exception.Message);
    }

    [Fact]
    public void ChangeEvent_WhenEventDoesNotExist_ThrowsException()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IManager>();

        var nonExistentEvent = new Event
        {
            EventId = 999,
            EventName = "Non-existent Event",
            EventDate = DateTime.Today,
            TicketPrice = 50.00m,
            EventDescription = "This event does not exist",
            Category = EventCategory.Music,
            UserId = "user123"
        };

        Assert.Throws<DbUpdateConcurrencyException>(() => manager.ChangeEvent(nonExistentEvent));
    }

    #endregion
}
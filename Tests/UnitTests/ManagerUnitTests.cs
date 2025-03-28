using Moq;
using Xunit;
using EM.BL;
using EM.BL.Domain;
using EM.DAL;

namespace Tests.UnitTests;

public class ManagerUnitTests
{
    [Fact]
    public void AddEvent_GivenValidData_CreatesEventAndReturnsIt()
    {
        // Arrange
        string name = "Test Event";
        DateTime date = DateTime.Today.AddDays(5);
        decimal? ticketPrice = 25.00m;
        string description = "A test event";
        EventCategory category = EventCategory.Music;
        string userId = "user123";

        var mockRepository = new Mock<IRepository>();
        var manager = new Manager(mockRepository.Object);

        // Set up repository mock to expect the CreateEvent call
        mockRepository.Setup(repo => repo.CreateEvent(It.IsAny<Event>())).Verifiable();

        // Act
        Event createdEvent = manager.AddEvent(name, date, ticketPrice, description, category, userId);

        // Assert
        Assert.NotNull(createdEvent);
        Assert.Equal(name, createdEvent.EventName);
        Assert.Equal(date, createdEvent.EventDate);
        Assert.Equal(ticketPrice, createdEvent.TicketPrice);
        Assert.Equal(description, createdEvent.EventDescription);
        Assert.Equal(category, createdEvent.Category);
        Assert.Equal(userId, createdEvent.UserId);

        // Verify that CreateEvent was called once
        mockRepository.Verify(repo => repo.CreateEvent(It.IsAny<Event>()), Times.Once());
    }

   

    [Fact]
    public void ChangeEvent_GivenValidEvent_UpdatesEvent()
    {
        // Arrange
        var evt = new Event
        {
            EventId = 1,
            EventName = "Updated Event",
            EventDate = DateTime.Today.AddDays(10),
            TicketPrice = 30.00m,
            EventDescription = "Updated description",
            Category = EventCategory.Sport,
            UserId = "user123"
        };

        var existingEvent = new Event
        {
            EventId = 1,
            EventName = "Original Event",
            EventDate = DateTime.Today.AddDays(5),
            TicketPrice = 25.00m,
            EventDescription = "Original description",
            Category = EventCategory.Music,
            UserId = "user123"
        };

        var mockRepository = new Mock<IRepository>();
        mockRepository.Setup(repo => repo.ReadEvent(1)).Returns(existingEvent);
        mockRepository.Setup(repo => repo.UpdateEvent(It.IsAny<Event>())).Verifiable();

        var manager = new Manager(mockRepository.Object);

        // Act
        manager.ChangeEvent(evt);

        // Assert
        // Verify that UpdateEvent was called once
        mockRepository.Verify(repo => repo.UpdateEvent(It.IsAny<Event>()), Times.Once());
    }

   
}
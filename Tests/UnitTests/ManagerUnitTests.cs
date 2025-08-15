using System;
using System.ComponentModel.DataAnnotations;
using Moq;
using Xunit;

using EM.BL;
using EM.BL.Domain;
using EM.DAL;

namespace Tests.UnitTests
{
    public class ManagerUnitTests
    {
        // ---------- AddEvent ----------

        [Fact]
        public void AddEvent_ShouldCreateAndReturnEvent_WhenInputValid()
        {
            // Arrange
            var repo = new Mock<IRepository>(MockBehavior.Strict);
            repo.Setup(r => r.CreateEvent(It.Is<Event>(e =>
                e.EventName == "Test Event" &&
                e.TicketPrice == 25m &&
                e.Category == EventCategory.Music &&
                e.UserId == "user123"
            ))).Verifiable();

            var sut = new Manager(repo.Object);

            // Act
            var created = sut.AddEvent(
                name: "Test Event",
                date: DateTime.Today.AddDays(5),
                ticketPrice: 25m,
                description: "A test event",
                category: EventCategory.Music,
                userId: "user123");

            // Assert
            Assert.NotNull(created);
            Assert.Equal("Test Event", created.EventName);
            Assert.Equal(25m, created.TicketPrice);
            Assert.Equal(EventCategory.Music, created.Category);
            Assert.Equal("user123", created.UserId);
            repo.VerifyAll(); // verification: CreateEvent exact één keer met juiste entity
        }

        [Fact]
        public void AddEvent_ShouldThrowValidationException_WhenNameMissing()
        {
            // Arrange
            var repo = new Mock<IRepository>(MockBehavior.Strict);
            var sut  = new Manager(repo.Object);

            // Act
            var ex = Assert.Throws<ValidationException>(() =>
                sut.AddEvent(
                    name: "",
                    date: DateTime.Today.AddDays(5),
                    ticketPrice: 25m,
                    description: "desc",
                    category: EventCategory.Music,
                    userId: "u"));

            // Assert
            Assert.Contains("EventName", ex.Message);
            repo.Verify(r => r.CreateEvent(It.IsAny<Event>()), Times.Never()); // verification: niet aangeroepen
        }

        // ---------- ChangeEvent ----------

        [Fact]
        public void ChangeEvent_ShouldCallRepositoryUpdate_WhenInputValid()
        {
            // Arrange
            var evt = new Event
            {
                EventId = 1,
                EventName = "Updated Event",
                EventDate = DateTime.Today.AddDays(10),
                TicketPrice = 30m,
                EventDescription = "Updated",
                Category = EventCategory.Sport,
                UserId = "user123"
            };

            var repo = new Mock<IRepository>(MockBehavior.Strict);
            repo.Setup(r => r.UpdateEvent(It.Is<Event>(e =>
                e.EventId == 1 &&
                e.EventName == "Updated Event" &&
                e.TicketPrice == 30m &&
                e.Category == EventCategory.Sport &&
                e.UserId == "user123"
            ))).Verifiable();

            var sut = new Manager(repo.Object);

            // Act
            sut.ChangeEvent(evt);

            // Assert
            repo.VerifyAll(); // verification: UpdateEvent exact één keer met juiste entity
        }

        [Fact]
        public void ChangeEvent_ShouldThrowValidationException_WhenNameMissing()
        {
            // Arrange
            var evt = new Event
            {
                EventId = 1,
                EventName = "", // invalid
                EventDate = DateTime.Today.AddDays(10),
                TicketPrice = 30m,
                EventDescription = "Updated",
                Category = EventCategory.Sport,
                UserId = "user123"
            };

            var repo = new Mock<IRepository>(MockBehavior.Strict);
            var sut  = new Manager(repo.Object);

            // Act
            var ex = Assert.Throws<ValidationException>(() => sut.ChangeEvent(evt));

            // Assert
            Assert.Contains("EventName", ex.Message);
            repo.Verify(r => r.UpdateEvent(It.IsAny<Event>()), Times.Never()); // verification: niet aangeroepen
        }
    }
}

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

using EM.BL;
using EM.BL.Domain;
using EM.UI.MVC.Controllers;

namespace Tests.UnitTests
{
    public class EventControllerUnitTests
    {
        [Fact]
        public async Task Add_ShouldRedirectToDetails_WhenModelValidAndUserFound()
        {
            // Arrange
            var mockManager = new Mock<IManager>(MockBehavior.Strict);
            var mockUserMgr = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var controller = new EventController(mockManager.Object, mockUserMgr.Object);

            var newEvent = new Event
            {
                EventName = "New Event",
                EventDate = DateTime.Today.AddDays(10),
                TicketPrice = 30m,
                EventDescription = "A new event",
                Category = EventCategory.Music
            };

            var userId = "user123";
            var identityUser = new IdentityUser { Id = userId };

            // simulate authenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            mockUserMgr.Setup(um => um.GetUserAsync(user)).ReturnsAsync(identityUser);
            mockManager.Setup(m => m.AddEvent(
                newEvent.EventName,
                newEvent.EventDate,
                newEvent.TicketPrice,
                newEvent.EventDescription,
                newEvent.Category,
                userId
            )).Returns(new Event { EventId = 1, EventName = newEvent.EventName });

            // Act
            var result = await controller.Add(newEvent);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal(1, redirect.RouteValues["id"]);
            mockManager.VerifyAll(); // verification: AddEvent exact één keer met juiste args
        }

        [Fact]
        public async Task Add_ShouldReturnViewWithModelError_WhenCategoryInvalid()
        {
            // Arrange
            var mockManager = new Mock<IManager>(MockBehavior.Strict);
            var mockUserMgr = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var controller = new EventController(mockManager.Object, mockUserMgr.Object);

            var newEvent = new Event
            {
                EventName = "New Event",
                EventDate = DateTime.Today.AddDays(10),
                TicketPrice = 30m,
                EventDescription = "A new event",
                Category = 0 // invalid
            };

            var userId = "user123";
            var identityUser = new IdentityUser { Id = userId };
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            mockUserMgr.Setup(um => um.GetUserAsync(user)).ReturnsAsync(identityUser);

            // Act
            var result = await controller.Add(newEvent);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Event>(view.Model);
            Assert.Equal(newEvent, model);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("Category"));
            Assert.Equal("Please select a valid category.", controller.ModelState["Category"].Errors[0].ErrorMessage);

            mockManager.Verify(m => m.AddEvent(
                It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<decimal?>(),
                It.IsAny<string>(), It.IsAny<EventCategory>(), It.IsAny<string>()),
                Times.Never()); // verification: manager niet aangeroepen
        }

        [Fact]
        public async Task Add_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var mockManager = new Mock<IManager>(MockBehavior.Strict);
            var mockUserMgr = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var controller = new EventController(mockManager.Object, mockUserMgr.Object);

            var newEvent = new Event
            {
                EventName = "New Event",
                EventDate = DateTime.Today.AddDays(10),
                TicketPrice = 30m,
                EventDescription = "A new event",
                Category = EventCategory.Music
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, "user123") }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            mockUserMgr.Setup(um => um.GetUserAsync(user)).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await controller.Add(newEvent);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User not found in the database.", unauthorized.Value);

            mockManager.Verify(m => m.AddEvent(
                It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<decimal?>(),
                It.IsAny<string>(), It.IsAny<EventCategory>(), It.IsAny<string>()),
                Times.Never()); // verification: manager niet aangeroepen
        }

        [Fact]
        public async Task Add_ShouldReturnUnauthorized_WhenAnonymous()
        {
            // Arrange
            var mockManager = new Mock<IManager>(MockBehavior.Strict);
            var mockUserMgr = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var controller = new EventController(mockManager.Object, mockUserMgr.Object);

            // niet ingelogd → lege identity (IsAuthenticated=false)
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var newEvent = new Event
            {
                EventName = "E",
                EventDate = DateTime.Today.AddDays(1),
                TicketPrice = 10m,
                EventDescription = "desc",
                Category = EventCategory.Music // geldig, zodat we de Unauthorized-tak raken
            };

            // Act
            var result = await controller.Add(newEvent);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User is not authenticated.", unauthorized.Value);
            mockManager.Verify(m => m.AddEvent(
                It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<decimal?>(),
                It.IsAny<string>(), It.IsAny<EventCategory>(), It.IsAny<string>()),
                Times.Never()); // verification: manager niet aangeroepen
        }
    }
}

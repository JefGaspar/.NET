using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using EM.BL;
using EM.BL.Domain;
using EM.UI.MVC.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;

namespace Tests.UnitTests;

public class EventControllerUnitTests
{
    [Fact]
    public async Task Add_GivenValidEvent_RedirectsToDetails()
    {
        // Arrange
        var mockManager = new Mock<IManager>();
        var mockUserManager = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        
        var controller = new EventController(mockManager.Object, mockUserManager.Object);

        var newEvent = new Event
        {
            EventName = "New Event",
            EventDate = DateTime.Today.AddDays(10),
            TicketPrice = 30.00m,
            EventDescription = "A new event",
            Category = EventCategory.Music
        };

        var userId = "user123";
        var identityUser = new IdentityUser { Id = userId };

        // Simuleer een ingelogde gebruiker
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Simuleer een geldige ModelState
        controller.ModelState.Clear();

        // Mock de user manager en manager
        mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(identityUser);
        mockManager.Setup(m => m.AddEvent(
            newEvent.EventName,
            newEvent.EventDate,
            newEvent.TicketPrice,
            newEvent.EventDescription,
            newEvent.Category,
            userId
        )).Returns(new Event { EventId = 1, EventName = newEvent.EventName }); // Simuleer een toegevoegd event

        // Act
        var result = await controller.Add(newEvent);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(1, redirectResult.RouteValues["id"]);

        // Verify that AddEvent was called once
        mockManager.Verify(m => m.AddEvent(
            newEvent.EventName,
            newEvent.EventDate,
            newEvent.TicketPrice,
            newEvent.EventDescription,
            newEvent.Category,
            userId
        ), Times.Once());
    }

    [Fact]
    public async Task Add_GivenInvalidCategory_ReturnsView()
    {
        // Arrange
        var mockManager = new Mock<IManager>();
        var mockUserManager = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        
        var controller = new EventController(mockManager.Object, mockUserManager.Object);

        var newEvent = new Event
        {
            EventName = "New Event",
            EventDate = DateTime.Today.AddDays(10),
            TicketPrice = 30.00m,
            EventDescription = "A new event",
            Category = 0 // Ongeldige categorie
        };

        var userId = "user123";
        var identityUser = new IdentityUser { Id = userId };

        // Simuleer een ingelogde gebruiker
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Mock de user manager
        mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(identityUser);

        // Act
        var result = await controller.Add(newEvent);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Event>(viewResult.Model);
        Assert.Equal(newEvent, model);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("Category"));
        Assert.Equal("Please select a valid category.", controller.ModelState["Category"].Errors[0].ErrorMessage);

        // Verify that AddEvent was not called
        mockManager.Verify(m => m.AddEvent(
            It.IsAny<string>(),
            It.IsAny<DateTime>(),
            It.IsAny<decimal?>(),
            It.IsAny<string>(),
            It.IsAny<EventCategory>(),
            It.IsAny<string>()
        ), Times.Never());
    }

    

    
    [Fact]
    public async Task Add_GivenUserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var mockManager = new Mock<IManager>();
        var mockUserManager = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        
        var controller = new EventController(mockManager.Object, mockUserManager.Object);

        var newEvent = new Event
        {
            EventName = "New Event",
            EventDate = DateTime.Today.AddDays(10),
            TicketPrice = 30.00m,
            EventDescription = "A new event",
            Category = EventCategory.Music
        };

        var userId = "user123";

        // Simuleer een ingelogde gebruiker
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Mock de user manager om geen gebruiker te vinden
        mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync((IdentityUser)null);

        // Act
        var result = await controller.Add(newEvent);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("User not found in the database.", unauthorizedResult.Value);

        // Verify that AddEvent was not called
        mockManager.Verify(m => m.AddEvent(
            It.IsAny<string>(),
            It.IsAny<DateTime>(),
            It.IsAny<decimal?>(),
            It.IsAny<string>(),
            It.IsAny<EventCategory>(),
            It.IsAny<string>()
        ), Times.Never());
    }
}
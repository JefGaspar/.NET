using EM.BL;
using EM.BL.Domain;
using EM.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EM.UI.MVC.Controllers;

public class EventController : Controller
{
    private readonly IManager _manager;
    private readonly UserManager<IdentityUser> _userManager;

    public EventController(IManager manager, UserManager<IdentityUser> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }
    
    public IActionResult Index()
    {
        var events = _manager.GetAllEventsWithOrganisation();
        return View(events);
    }
    [HttpGet]
    [Authorize]
    public IActionResult Add()
    {
        return View(); // Render de Add-view
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Event newEvent)
    {
        if (newEvent.Category == 0) // Controleer of geen geldige categorie is geselecteerd
        {
            ModelState.AddModelError("Category", "Please select a valid category.");
        }

        if (!ModelState.IsValid)
        {
            return View(newEvent);
        }

        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized("User is not authenticated.");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized("User not found in the database.");
        }

        var addedEvent = _manager.AddEvent(
            newEvent.EventName,
            newEvent.EventDate,
            newEvent.TicketPrice,
            newEvent.EventDescription,
            newEvent.Category,
            currentUser.Id // Pass the UserId directly
        );

        return RedirectToAction("Details", new { id = addedEvent.EventId });
    }
    public async Task<IActionResult> Details(int id)
    {
        // Use GetEventWithVisitors to fetch the event with tickets and visitors
        var eventEntity = _manager.GetEventWithVisitors(id);
        if (eventEntity == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(eventEntity.UserId);
        var viewModel = new EventDetailsViewModel
        {
            EventId = eventEntity.EventId,
            EventName = eventEntity.EventName,
            EventDescription = eventEntity.EventDescription,
            Category = eventEntity.Category,
            EventDate = eventEntity.EventDate,
            TicketPrice = eventEntity.TicketPrice,
            MaintainedByEmail = user?.Email ?? "Unknown",
            MaintainedByUserId = eventEntity.UserId, // Stel de UserId in
            Tickets = eventEntity.Tickets?.Select(t => new TicketViewModel
            {
                VisitorId = t.Visitor.VisitorId,
                FirstName = t.Visitor.FirstName,
                LastName = t.Visitor.LastName,
                PurchaseDate = t.PurchaseDate,
                PaymentMethode = t.PaymentMethode
            }).ToList() ?? new List<TicketViewModel>()
        };

        return View(viewModel);
    }

}
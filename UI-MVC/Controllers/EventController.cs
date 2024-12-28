using EM.BL;
using EM.BL.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EM.UI.MVC.Controllers;

public class EventController : Controller
{
    private readonly IManager _manager;

    public EventController(IManager manager)
    {
        _manager = manager;
    }
    
    public IActionResult Index()
    {
        var events = _manager.GetAllEventsWithOrganisation();
        return View(events);
    }
    [HttpGet]
    public IActionResult Add()
    {
        return View(); // Render de Add-view
    }
    [HttpPost]
    public IActionResult Add(Event newEvent)
    {
        if (newEvent.Category == 0) // Controleer of geen geldige categorie is geselecteerd
        {
            ModelState.AddModelError("Category", "Please select a valid category.");
        }

        if (!ModelState.IsValid)
        {
            // Toon het formulier opnieuw met validatiefouten
            return View(newEvent);
        }

        var addedEvent = _manager.AddEvent(
            newEvent.EventName,
            newEvent.EventDate,
            newEvent.TicketPrice,
            newEvent.EventDescription,
            newEvent.Category
        );

        return RedirectToAction("Details", new { id = addedEvent.EventId });
    }

    public IActionResult Details(int id)
    {
        var evnt = _manager.GetEventWithVisitors(id);
        return View(evnt);
    }

}
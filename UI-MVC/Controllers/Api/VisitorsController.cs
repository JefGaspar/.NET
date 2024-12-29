using EM.BL;
using EM.BL.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EM.UI.MVC.Controllers.Api;
[Route("api/[controller]")]
[ApiController]
public class VisitorsController: ControllerBase //base lichter dan Controller, in contoller zit nog van alles over MVC(httpp) wat in API niet nodig is
{
    private readonly IManager _manager;

    public VisitorsController(IManager manager)
    {
        _manager = manager;
    }
    
    [HttpGet("{visitorId}/Events")]
    public IActionResult GetEventsByVisitor(int visitorId)
    {
        var events = _manager.GetEventsByVisitor(visitorId);
        return Ok(events);// Retourneer lege lijst als er geen evenementen zijn
    }

    [HttpGet("{visitorId}/AvailableEvents")]
    public IActionResult GetAvailableEventsForVisitor(int visitorId)
    {
            var availableEvents = _manager.GetAvailableEventsForVisitor(visitorId);
            return Ok(availableEvents);// Retourneer altijd, ook als de lijst leeg is
    }




}
using EM.BL;
using EM.BL.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EM.UI.MVC.Controllers.Api;
[Route("api/[controller]")]
[ApiController]
public class VisitorsController: ControllerBase
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
        if (events == null || !events.Any())
        {
            return NotFound();
        }
        return Ok(events);
    }

    [HttpGet("{visitorId}/AvailableEvents")]
    public IActionResult GetAvailableEventsForVisitor(int visitorId)
    {
            var availableEvents = _manager.GetAvailableEventsForVisitor(visitorId);
            if (availableEvents == null || !availableEvents.Any())
            {
                return NotFound();
            }
            return Ok(availableEvents);
    }




}
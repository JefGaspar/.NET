using EM.BL;
using EM.UI.MVC.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EM.UI.MVC.Controllers.Api;
[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly IManager _manager;

    public TicketsController(IManager manager)
    {
        _manager = manager;
    }
    
    [Authorize]
    [HttpPost]
    public IActionResult AddTicket(TicketDto ticketDto)
    {
            // Voeg het ticket toe via de Manager
            var ticket = _manager.AddTicket(ticketDto.EventId, ticketDto.VisitorId, ticketDto.PaymentMethod);

            // Maak een DTO van het aangemaakte ticket, om circulatie te voorkomen. kijk laatste les min 14
            var ticketResponseDto = new TicketDto
            {
                VisitorId = ticket.Visitor.VisitorId,
                EventId = ticket.Event.EventId,
                PurchaseDate = ticket.PurchaseDate,
                PaymentMethod = ticket.PaymentMethode
            };

            return Ok(ticketResponseDto);
    }

    
    
}


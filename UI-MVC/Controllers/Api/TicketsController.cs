using EM.BL;
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
    
    [HttpPost]
    public IActionResult AddTicket([FromBody] TicketDto ticketDto)
    {
        try
        {
            // Voeg het ticket toe via de Manager
            var ticket = _manager.AddTicket(ticketDto.EventId, ticketDto.VisitorId, ticketDto.PaymentMethod);

            // Maak een DTO van het aangemaakte ticket
            var ticketResponseDto = new TicketDto
            {
                VisitorId = ticket.Visitor.VisitorId,
                EventId = ticket.Event.EventId,
                PurchaseDate = ticket.PurchaseDate,
                PaymentMethod = ticket.PaymentMethode
            };

            return Ok(ticketResponseDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    
    
}


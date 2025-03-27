using System.Security.Claims;
using EM.BL;
using EM.BL.Domain;
using EM.UI.MVC.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace EM.UI.MVC.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IManager _manager;
        private readonly UserManager<IdentityUser> _userManager;

        public EventsController(IManager manager, UserManager<IdentityUser> userManager)
        {
            _manager = manager;
            _userManager = userManager;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventPriceDto model)
        {
            if (model == null || model.TicketPrice == null)
            {
                return BadRequest("Ticket price is required.");
            }

            var eventEntity = _manager.GetEvent(id);
            if (eventEntity == null)
            {
                return NotFound("Event not found.");
            }

            // Controleer of de huidige gebruiker de eigenaar is of een Admin
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (eventEntity.UserId != userId && !isAdmin)
            {
                return Forbid(); // Alleen de eigenaar of een Admin kan het event bewerken
            }

            eventEntity.TicketPrice = model.TicketPrice;
            _manager.ChangeEvent(eventEntity);

            // Retourneer een JSON-respons, zoals in het andere project
            return Ok(new { success = true, newPrice = model.TicketPrice });
        }
    }
}
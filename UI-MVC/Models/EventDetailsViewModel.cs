using EM.BL.Domain;

namespace EM.UI.MVC.Models
{
    public class EventDetailsViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public EventCategory Category { get; set; }
        public DateTime EventDate { get; set; }
        public decimal? TicketPrice { get; set; }
        public string MaintainedByEmail { get; set; }
        public string MaintainedByUserId { get; set; } // Nieuwe eigenschap voor vergelijking
        public List<TicketViewModel> Tickets { get; set; } 
    }
}
namespace EM.UI.MVC.Models;

public class EventDto
{
    public int EventId { get; set; }
    public string EventName { get; set; }
    public DateTime EventDate { get; set; }
    public decimal? TicketPrice { get; set; }
    public string Category { get; set; }
}
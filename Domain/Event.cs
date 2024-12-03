using System.ComponentModel.DataAnnotations;

namespace UI;

public class Event
{
    public int EventId { get; set; }   
    
    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(100, ErrorMessage = "Event name cannot exceed 100 characters.")]
    public string EventName { get; set; }
    
    [Required(ErrorMessage = "Event description is required.")]
    [StringLength(500, ErrorMessage = "Event description cannot exceed 500 characters.")]
    public string EventDescription { get; set; }
    public EventCategory Category { get; set; }
    
    [Required(ErrorMessage = "Event date is required.")]
    public DateTime EventDate { get; set; }
    
    [Range(0, 1000, ErrorMessage = "Ticket price must be between 0 and 1000.")]
    public decimal? TicketPrice { get; set; }
    
    //kan ICollection zijn ipv list omdat het voor een veel relatie
    //List geeft een harde koppeling omdat het uitgangspunt index based is wat niet nodig is voor een nav property
    public ICollection<Visitor>  Visitors { get; set; } = new List<Visitor>();
    public Organisation Organisation { get; set; }
 
    public Event() { }

    public Event(int eventId, string eventName, string eventDescription,  DateTime eventDate, decimal? ticketPrice, List<Visitor> visitors, EventCategory category = EventCategory.Sport)
    {
        this.EventId = eventId;
        this.EventName = eventName;
        this.EventDescription = eventDescription;
        this.Category = category;
        this.EventDate = eventDate;
        this.TicketPrice = ticketPrice;
        this.Visitors = visitors;
    }

    public string GetInfo()
    {
        return ToString();
    }

    
}


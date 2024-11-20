namespace UI;

public class Event
{
    public int EventId { get; set; }   
    public string EventName { get; set; }
    public string EventDescription { get; set; }
    public EventCategory Category { get; set; }
    public DateTime EventDate { get; set; }
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


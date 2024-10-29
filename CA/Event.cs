namespace CA;

public class Event
{
    public int EventId { get; set; }   
    public string EventName { get; set; }
    public string EventDescription { get; set; }
    public EventCategory Category { get; set; }
    public DateTime EventDate { get; set; }
    public decimal? TicketPrice { get; set; }
    public List<Visitor>  Visitors { get; set; } = new List<Visitor>();
    public Organisation Organisation { get; set; }
    
    public override string ToString()
    {
        string ticketPriceStr = TicketPrice.HasValue ? $"€{TicketPrice.Value:F2}" : "Free";
        string description = EventDescription ?? "No description available";

        return $"{EventName}: Date: {EventDate:yyyy-MM-dd HH:mm}, Description: {description}, Price: {ticketPriceStr}, Category: {Category}";
    }

    public Event() { }

    public Event(int eventId, string eventName, string eventDescription,  DateTime eventDate, decimal? ticketPrice, List<Visitor> visitors, EventCategory category = EventCategory.sport)
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


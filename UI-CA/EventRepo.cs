namespace UI;

public class EventRepo
{
    private List<Event> events;

    public EventRepo(List<Event> events)
    {
        this.events = events;
    }

    public List<Event> GetAllEvents()
    {
        return events;
    }

    public List<Event> GetEventsByCategory(EventCategory category)
    {
        return events.Where(e => e.Category == category).ToList();
    }
}
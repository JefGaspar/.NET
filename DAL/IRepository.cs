using UI;

namespace EM.DAL;

public interface IRepository
{
    public Event ReadEvent(int id);
    public IEnumerable<Event> ReadAllEvents();
    public IEnumerable<Event> ReadEventsByCategory(EventCategory category);
    public void CreateEvent(Event evnt);
    public Visitor ReadVisitor(int id);
    public IEnumerable<Visitor> ReadAllVisitors();
    public IEnumerable<Visitor> ReadVisitorsByNameOrCity(string firstName, string city);
    public void CreateVisitor(Visitor visitor);
}
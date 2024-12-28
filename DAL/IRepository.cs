using EM.BL.Domain;

namespace EM.DAL;

public interface IRepository
{
    public Event ReadEvent(int id);
    public Event ReadEventWithVisitors(int id);
    public IEnumerable<Event> ReadAllEvents();
    public IEnumerable<Event> ReadEventsByCategory(EventCategory category);
    public void CreateEvent(Event evnt);
    public Visitor ReadVisitor(int id);
    public IEnumerable<Visitor> ReadAllVisitors();
    public IEnumerable<Visitor> ReadVisitorsByNameOrCity(string firstName, string city);
    public IEnumerable<Event> ReadEventsByVisitor(int visitorId);

    public void CreateVisitor(Visitor visitor);
    public void CreateOrganisation(Organisation organisation);
    public IEnumerable<Event> ReadAllEventsWithOrganisation();
    public IEnumerable<Visitor> ReadAllVisitorsWithEvents();
    public void CreateTicket(Ticket ticket);
    public Ticket GetTicket(int eventId, int visitorId);
    public void DeleteTicket(int eventId, int visitorId);
    public IEnumerable<Event> ReadEventsOfVisitor(int visitorId);
    IEnumerable<Organisation> ReadAllOrganisations();

}
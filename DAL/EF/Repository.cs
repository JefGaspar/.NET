using EM.BL.Domain;
using Microsoft.EntityFrameworkCore;

namespace EM.DAL.EF;

public class Repository : IRepository
{
    private readonly EmDbContext _emDbContext;

    public Repository(EmDbContext context)
    {
        _emDbContext = context;
    }


    public Event ReadEvent(int id)
    {
        return _emDbContext.Events.Find(id);
    }

    public Event ReadEventWithVisitors(int id)
    {
        return _emDbContext.Events
            .Include(e => e.Tickets)
            .ThenInclude(t => t.Visitor)
            .SingleOrDefault(e => e.EventId == id);
    }
    public IEnumerable<Event> ReadAllEvents()
    {
        return _emDbContext.Events.ToList();
    }

    public IEnumerable<Event> ReadEventsByCategory(EventCategory category)
    {
        return _emDbContext.Events
            .Where(e => e.Category == category)
            .ToList();
    }

    public void CreateEvent(Event evnt)
    {
        _emDbContext.Events.Add(evnt);
        _emDbContext.SaveChanges();
    }

    public Visitor ReadVisitor(int id)
    {
        return _emDbContext.Visitors.Find(id);
    }


    public IEnumerable<Visitor> ReadAllVisitors()
    {
        return _emDbContext.Visitors.ToList();
    }

/*
 * parameters zijn optioneel gemaakt omdat niet beide filters ingevult moeten zijn, dus al gebruiker nu leeg laat krijgt standaardwaarde van null
 */
    public IEnumerable<Visitor> ReadVisitorsByNameOrCity(string firstName = null, string city= null)
    {
        var query = _emDbContext.Visitors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            query = query.Where(v => v.FirstName.ToLower().Contains(firstName.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(v => v.City.ToLower().Contains(city.ToLower()));
        }
        return query.ToList();
    }

    public void CreateVisitor(Visitor visitor)
    {
        _emDbContext.Visitors.Add(visitor);
        _emDbContext.SaveChanges();
    }

    public void CreateOrganisation(Organisation organisation)
    {
        _emDbContext.Organisations.Add(organisation);
        _emDbContext.SaveChanges();
    }

    public IEnumerable<Event> ReadAllEventsWithOrganisation()
    {
        return _emDbContext.Events
            .Include(e => e.Organisation) // Eager load de organisatie van elk event
            .ToList();    
    }

    public IEnumerable<Visitor> ReadAllVisitorsWithEvents()
    {
        return _emDbContext.Visitors
            .Include(v => v.Tickets) // Laad de tickets van elke visitor
            .ThenInclude(t => t.Event) // Laad de evenementen via de tickets
            .ToList();
        
    }
    
    public IEnumerable<Event> ReadEventsByVisitor(int visitorId)
    {
        return _emDbContext.Tickets
            .Where(t => t.Visitor.VisitorId == visitorId)
            .Select(t => t.Event)
            .ToList();
    }


    public void CreateTicket(Ticket ticket)
    {
        _emDbContext.Tickets.Add(ticket);
        _emDbContext.SaveChanges();
    }

    public Ticket GetTicket(int eventId, int visitorId)
    {
        return _emDbContext.Tickets.Find(eventId, visitorId);    
    }

    public void DeleteTicket(int eventId, int visitorId)
    {
        var ticket = _emDbContext.Tickets.Find(eventId, visitorId);
        _emDbContext.Tickets.Remove(ticket);
        _emDbContext.SaveChanges();
    }

    public IEnumerable<Event> ReadEventsOfVisitor(int visitorId)
    {
        return _emDbContext.Tickets
            .Where(t => t.Visitor.VisitorId == visitorId)
            .Select(t => t.Event)
            .ToList();    }

    public IEnumerable<Organisation> ReadAllOrganisations()
    {
        return _emDbContext.Organisations.ToList();
    }

}

 
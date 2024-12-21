using Microsoft.EntityFrameworkCore;
using UI;

namespace EM.DAL.EF;

public class Repository : IRepository
{
    private readonly EMDbContext _emDbContext;

    public Repository(EMDbContext context)
    {
        _emDbContext = context;
    }


    public Event ReadEvent(int id)
    {
        return _emDbContext.Events
            .Include(e => e.Visitors)
            .SingleOrDefault(e => e.EventId == id);
    }

    public IEnumerable<Event> ReadAllEvents()
    {
        return _emDbContext.Events
            .Include(e => e.Visitors)
            .ToList();
    }

    public IEnumerable<Event> ReadEventsByCategory(EventCategory category)
    {
        return _emDbContext.Events
            .Where(e => e.Category == category).
            Include(e => e.Visitors)
            .ToList();
    }

    public void CreateEvent(Event evnt)
    {
        _emDbContext.Events.Add(evnt);
        _emDbContext.SaveChanges();
    }

    public Visitor ReadVisitor(int id)
    {
        return _emDbContext.Visitors
            .Include(v => v.Events) 
            .SingleOrDefault(v => v.VisitorId == id);
    }


    public IEnumerable<Visitor> ReadAllVisitors()
    {
        return _emDbContext.Visitors
            .Include(v => v.Events) 
            .ToList();
    }


    public IEnumerable<Visitor> ReadVisitorsByNameOrCity(string firstName, string city)
    {
        return _emDbContext.Visitors.Where(v =>
            (string.IsNullOrEmpty(firstName) || v.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(city) || v.City.Contains(city, StringComparison.OrdinalIgnoreCase))
        ).ToList();    }

    public void CreateVisitor(Visitor visitor)
    {
        _emDbContext.Visitors.Add(visitor);
        _emDbContext.SaveChanges();
        
    }
}
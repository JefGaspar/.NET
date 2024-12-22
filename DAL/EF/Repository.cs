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
        return _emDbContext.Events.Find(id);
    }

    public IEnumerable<Event> ReadAllEvents()
    {
        return _emDbContext.Events.ToList();
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
}

 
namespace EM.DAL;
/*
public class InMemoryRepository : IRepository
{
    private static List<Event> _events = new List<Event>();
    private static List<Visitor> _visitors = new List<Visitor>();

    public static void Seed()
    {
        _events = new List<Event>
        {
            new Event
            {
                EventId = 1,
                EventName = "Studay",
                EventDate = new DateTime(2024, 09, 18, 14, 00, 00),
                TicketPrice = null,
                EventDescription = "Party for students",
                Category = EventCategory.Festival
            },
            new Event
            {
                EventId = 2,
                EventName = "Tech Expo",
                EventDate = new DateTime(2024, 11, 05, 10, 00, 00),
                TicketPrice = 25.00m,
                EventDescription = "A showcase of the latest in technology",
                Category = EventCategory.Conference
            },
            new Event
            {
                EventId = 3,
                EventName = "Business Summit",
                EventDate = new DateTime(2024, 10, 10, 09, 00, 00),
                TicketPrice = 100.00m,
                EventDescription = "Summit for business networking and learning",
                Category = EventCategory.Networking
            },
            new Event
            {
                EventId = 4,
                EventName = "Music Fest",
                EventDate = new DateTime(2024, 08, 25, 16, 00, 00),
                TicketPrice = 50.00m,
                EventDescription = "A weekend of music and fun",
                Category = EventCategory.Music
            }
        };

        _visitors = new List<Visitor>
        {
            new Visitor
            {
                VisitorId = 1,
                FirstName = "Stan",
                LastName = "Schins",
                Email = "stanschins@gmail.com",
                PhoneNumber = "0484710770",
                City = "Antwerpen"
            },
            new Visitor
            {
                VisitorId = 2,
                FirstName = "Emma",
                LastName = "Jones",
                Email = "emma.jones@example.com",
                PhoneNumber = "0475123456",
                City = "Gent"
            },
            new Visitor
            {
                VisitorId = 3,
                FirstName = "Lucas",
                LastName = "Baker",
                Email = "lucas.baker@example.com",
                PhoneNumber = "0476987654",
                City = "Brussel"
            },
            new Visitor
            {
                VisitorId = 4,
                FirstName = "Olivia",
                LastName = "Smith",
                Email = "olivia.smith@example.com",
                PhoneNumber = "0476112233",
                City = "Antwerpen"
            }
        };

        // Koppel bezoekers aan evenementen
        _events[0].Visitors.Add(_visitors[0]);
        _events[0].Visitors.Add(_visitors[1]);
        _events[1].Visitors.Add(_visitors[0]);
        _events[1].Visitors.Add(_visitors[2]);
        _events[2].Visitors.Add(_visitors[3]);
        _events[2].Visitors.Add(_visitors[1]);
        _events[3].Visitors.Add(_visitors[3]);
        _events[3].Visitors.Add(_visitors[2]);
    }

    public Event ReadEvent(int id)
    {
        return _events.Single(e => e.EventId == id);
    }

    public IEnumerable<Event> ReadAllEvents()
    {
        return _events;
    }

    public IEnumerable<Event> ReadEventsByCategory(EventCategory category)
    {
        return _events.Where(e => e.Category == category).ToList();
    }

    public void CreateEvent(Event evnt)
    {
        // Bepaal de hoogste EventId in de lijst en verhoog dit met 1, met count + 1 kon dit voor problemen zorgen als we gingen deleten en terug create
        //.Any controleert of er al iets in de lijst staat, als het leeg is wordt het ingesteld op 1
        // anders max opgehaald en 1 aan toegevoegd
        evnt.EventId = _events.Any() ? _events.Max(e => e.EventId) + 1 : 1;
        _events.Add(evnt);
    }

    public Visitor ReadVisitor(int id)
    {
        return _visitors.Single(v => v.VisitorId == id);
    }

    public IEnumerable<Visitor> ReadAllVisitors()
    {
        return _visitors;
    }

    public IEnumerable<Visitor> ReadVisitorsByNameOrCity(string firstName, string city)
    {
        return _visitors.Where(v =>
            (string.IsNullOrEmpty(firstName) || v.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(city) || v.City.Contains(city, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

    public void CreateVisitor(Visitor visitor)
    {
        visitor.VisitorId = _visitors.Any() ? _visitors.Max(v => v.VisitorId) + 1 : 1;
        _visitors.Add(visitor);
    }

}*/

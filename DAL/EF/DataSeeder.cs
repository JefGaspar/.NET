using UI;

namespace EM.DAL.EF;

public static class DataSeeder
{
    public static void Seed(EMDbContext context)
    {
         // Controleer of de database al gegevens bevat
        if (context.Events.Any() || context.Visitors.Any())
        {
            // Stop als er al gegevens zijn
            return;
        }
        
         // Voeg evenementen toe
        var events = new List<Event>
        {
            new Event
            {
                EventName = "Studay",
                EventDate = new DateTime(2024, 09, 18, 14, 00, 00),
                TicketPrice = null,
                EventDescription = "Party for students",
                Category = EventCategory.Festival
            },
            new Event
            {
                EventName = "Tech Expo",
                EventDate = new DateTime(2024, 11, 05, 10, 00, 00),
                TicketPrice = 25.00m,
                EventDescription = "A showcase of the latest in technology",
                Category = EventCategory.Conference
            },
            new Event
            {
                EventName = "Business Summit",
                EventDate = new DateTime(2024, 10, 10, 09, 00, 00),
                TicketPrice = 100.00m,
                EventDescription = "Summit for business networking and learning",
                Category = EventCategory.Networking
            },
            new Event
            {
                EventName = "Music Fest",
                EventDate = new DateTime(2024, 08, 25, 16, 00, 00),
                TicketPrice = 50.00m,
                EventDescription = "A weekend of music and fun",
                Category = EventCategory.Music
            }
        };

        // Voeg bezoekers toe
        var visitors = new List<Visitor>
        {
            new Visitor
            {
                FirstName = "Stan",
                LastName = "Schins",
                Email = "stanschins@gmail.com",
                PhoneNumber = "0484710770",
                City = "Antwerpen"
            },
            new Visitor
            {
                FirstName = "Emma",
                LastName = "Jones",
                Email = "emma.jones@example.com",
                PhoneNumber = "0475123456",
                City = "Gent"
            },
            new Visitor
            {
                FirstName = "Lucas",
                LastName = "Baker",
                Email = "lucas.baker@example.com",
                PhoneNumber = "0476987654",
                City = "Brussel"
            },
            new Visitor
            {
                FirstName = "Olivia",
                LastName = "Smith",
                Email = "olivia.smith@example.com",
                PhoneNumber = "0476112233",
                City = "Antwerpen"
            }
        };

        // Voeg bezoekers toe aan evenementen
        events[0].Visitors = new List<Visitor> { visitors[0], visitors[1] };
        events[1].Visitors = new List<Visitor> { visitors[0], visitors[2] };
        events[2].Visitors = new List<Visitor> { visitors[3], visitors[1] };
        events[3].Visitors = new List<Visitor> { visitors[3], visitors[2] };

        // Voeg gegevens toe aan de context
        context.Events.AddRange(events);
        context.Visitors.AddRange(visitors);

        // Sla wijzigingen op in de database
        context.SaveChanges();

        // Maak de change tracker leeg om de objecten los te koppelen
        context.ChangeTracker.Clear();
    }
}
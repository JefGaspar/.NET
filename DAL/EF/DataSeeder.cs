using UI;

namespace EM.DAL.EF;
public static class DataSeeder
{
    public static void Seed(EMDbContext context)
    {
        // Controleer of de database al gegevens bevat
        if (context.Events.Any() || context.Visitors.Any())
        {
            return; // Stop als er al gegevens zijn
        }

        // Voeg evenementen toe met constructors
        Event studay = new Event(1, "Studay", "Party for students", new DateTime(2024, 09, 18, 14, 00, 00), null, EventCategory.Festival);
        Event techExpo = new Event(2, "Tech Expo", "A showcase of the latest in technology", new DateTime(2024, 11, 05, 10, 00, 00), 25.00m, EventCategory.Conference);
        Event businessSummit = new Event(3, "Business Summit", "Summit for business networking and learning", new DateTime(2024, 10, 10, 09, 00, 00), 100.00m, EventCategory.Networking);
        Event musicFest = new Event(4, "Music Fest", "A weekend of music and fun", new DateTime(2024, 08, 25, 16, 00, 00), 50.00m, EventCategory.Music);

        // Voeg bezoekers toe met constructors
        Visitor stan = new Visitor(1, "Stan", "Schins", "stanschins@gmail.com", "0484710770", "Antwerpen");
        Visitor emma = new Visitor(2, "Emma", "Jones", "emma.jones@example.com", "0475123456", "Gent");
        Visitor lucas = new Visitor(3, "Lucas", "Baker", "lucas.baker@example.com", "0476987654", "Brussel");
        Visitor olivia = new Visitor(4, "Olivia", "Smith", "olivia.smith@example.com", "0476112233", "Antwerpen");

        Organisation techSphere = new Organisation(1, "TechSphere Innovations", "Leading provider of cutting-edge technology solutions, specializing in AI and cloud computing.", new DateOnly(2010, 4, 15), "contact@techsphere.com");
        Organisation greenFuture = new Organisation(2, "GreenFuture Initiatives", "A non-profit organization dedicated to promoting sustainability and renewable energy projects.", new DateOnly(2005, 8, 23), "info@greenfuture.org");
        Organisation culturalHorizons = new Organisation(3, "Cultural Horizons", "Fostering cultural exchange and creative arts through events and workshops.", new DateOnly(2018, 3, 9), "hello@culturalhorizons.com");
        Organisation healthLink = new Organisation(4, "HealthLink International", "Connecting healthcare providers with innovative medical technologies and solutions worldwide.", new DateOnly(2012, 11, 5), "support@healthlinkint.com");

        // Koppel bezoekers aan evenementen
        studay.Visitors.Add(stan);
        studay.Visitors.Add(emma);  

        techExpo.Visitors.Add(stan);
        techExpo.Visitors.Add(lucas);

        businessSummit.Visitors.Add(olivia);
        businessSummit.Visitors.Add(emma);

        musicFest.Visitors.Add(olivia);
        musicFest.Visitors.Add(lucas);

        // Voeg objecten toe aan de context
        context.Events.AddRange(studay, techExpo, businessSummit, musicFest);
        context.Visitors.AddRange(stan, emma, lucas, olivia);
        context.Organisations.AddRange(techSphere, greenFuture, culturalHorizons, healthLink);


        // Sla wijzigingen op in de database
        context.SaveChanges();

        // Maak de change tracker leeg
        context.ChangeTracker.Clear();
    }
}


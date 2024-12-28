using EM.BL.Domain;

namespace EM.DAL.EF;
public static class DataSeeder
{
    public static void Seed(EmDbContext context)
    {
        // Controleer of de database al gegevens bevat
        if (context.Events.Any() || context.Visitors.Any() || context.Tickets.Any())
        {
            return; // Stop als er al gegevens zijn
        }

        // Voeg evenementen toe met constructors
        Event studay = new Event("Studay", "Party for students", new DateTime(2024, 09, 18, 14, 00, 00), null, EventCategory.Festival);
        Event techExpo = new Event("Tech Expo", "A showcase of the latest in technology", new DateTime(2024, 11, 05, 10, 00, 00), 25.00m, EventCategory.Conference);
        Event businessSummit = new Event("Business Summit", "Summit for business networking and learning", new DateTime(2024, 10, 10, 09, 00, 00), 100.00m, EventCategory.Networking);
        Event musicFest = new Event("Music Fest", "A weekend of music and fun", new DateTime(2024, 08, 25, 16, 00, 00), 50.00m, EventCategory.Music);

        // Voeg bezoekers toe met constructors
        Visitor stan = new Visitor("Stan", "Schins", "stanschins@gmail.com", "0484710770", "Antwerpen");
        Visitor emma = new Visitor("Emma", "Jones", "emma.jones@gmail.com", "0475123456", "Gent");
        Visitor lucas = new Visitor("Lucas", "Baker", "lucas.baker@hotmail.com", "0476987654", "Brussel");
        Visitor olivia = new Visitor("Olivia", "Smith", "olivia.smith@gmail.com", "0476112233", "Antwerpen");

        Organisation techSphere = new Organisation("TechSphere Innovations", "Leading provider of cutting-edge technology solutions, specializing in AI and cloud computing.", new DateOnly(2010, 4, 15), "contact@techsphere.com");
        Organisation greenFuture = new Organisation("GreenFuture Initiatives", "A non-profit organization dedicated to promoting sustainability and renewable energy projects.", new DateOnly(2005, 8, 23), "info@greenfuture.org");
        Organisation culturalHorizons = new Organisation("Cultural Horizons", "Fostering cultural exchange and creative arts through events and workshops.", new DateOnly(2018, 3, 9), "hello@culturalhorizons.com");
        Organisation healthLink = new Organisation("HealthLink International", "Connecting healthcare providers with innovative medical technologies and solutions worldwide.", new DateOnly(2012, 11, 5), "support@healthlinkint.com");
        Organisation futureTech = new Organisation("FutureTech Solutions", "Innovative solutions provider specializing in robotics and automation for industries.", new DateOnly(2015, 6, 20), "info@futuretech.com");

        // Voeg tickets toe
        Ticket studayT856 = new Ticket { Event = studay, Visitor = stan, PurchaseDate = new DateTime(2024,06,22,15,22,36), PurchaseMethode = PurchaseMethode.Online};
        Ticket studayT711 = new Ticket { Event = studay, Visitor = emma, PurchaseDate = new DateTime(2024, 09, 18, 16, 32, 10), PurchaseMethode = PurchaseMethode.AtTheDoor};
        Ticket techExpoT522 = new Ticket { Event = techExpo, Visitor = lucas, PurchaseDate = new DateTime(2024, 11, 05, 10, 12, 55), PurchaseMethode = PurchaseMethode.AtTheDoor };
        Ticket musicFestT333 = new Ticket { Event = musicFest, Visitor = emma, PurchaseDate =  new DateTime(2024,1,19,8,8,10), PurchaseMethode = PurchaseMethode.Online };
        Ticket businessSummitT122 = new Ticket { Event = businessSummit, Visitor = emma, PurchaseDate = new DateTime(2024,10,9,18,28,15), PurchaseMethode = PurchaseMethode.Online };

        // Koppel organisaties aan evenementen
        studay.Organisation = techSphere;
        techExpo.Organisation = greenFuture;
        businessSummit.Organisation = culturalHorizons;
       

        // Voeg objecten toe aan de context
        context.Events.AddRange(studay, techExpo, businessSummit, musicFest);
        context.Visitors.AddRange(stan, emma, lucas, olivia);
        context.Organisations.AddRange(techSphere, greenFuture, culturalHorizons, healthLink, futureTech);
        context.Tickets.AddRange(studayT856, studayT711, techExpoT522, musicFestT333, businessSummitT122);

        // Sla wijzigingen op in de database
        context.SaveChanges();

        // Maak de change tracker leeg
        context.ChangeTracker.Clear();
    }
}


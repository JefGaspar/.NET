using EM.BL.Domain;
using Microsoft.AspNetCore.Identity;

namespace EM.DAL.EF
{
    public class DataSeeder
    {
        private readonly EmDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to inject dependencies
        public DataSeeder(EmDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            // Check if the database already contains data
            if (_context.Events.Any() || _context.Visitors.Any() || _context.Tickets.Any())
            {
                return; // Stop if data already exists
            }

            // Create events
            Event studay = new Event("Studay", "Party for students", new DateTime(2024, 09, 18, 14, 00, 00), null, EventCategory.Festival);
            Event techExpo = new Event("Tech Expo", "A showcase of the latest in technology", new DateTime(2024, 11, 05, 10, 00, 00), 25.00m, EventCategory.Conference);
            Event businessSummit = new Event("Business Summit", "Summit for business networking and learning", new DateTime(2024, 10, 10, 09, 00, 00), 100.00m, EventCategory.Networking);
            Event musicFest = new Event("Music Fest", "A weekend of music and fun", new DateTime(2024, 08, 25, 16, 00, 00), 50.00m, EventCategory.Music);

            // Create visitors
            Visitor stan = new Visitor("Stan", "Schins", "stanschins@gmail.com", "0484710770", "Antwerpen");
            Visitor emma = new Visitor("Emma", "Jones", "emma.jones@gmail.com", "0475123456", "Gent");
            Visitor lucas = new Visitor("Lucas", "Baker", "lucas.baker@hotmail.com", "0476987654", "Brussel");
            Visitor olivia = new Visitor("Olivia", "Smith", "olivia.smith@gmail.com", "0476112233", "Antwerpen");

            // Create organisations
            Organisation techSphere = new Organisation("TechSphere Innovations", "Leading provider of cutting-edge technology solutions, specializing in AI and cloud computing.", new DateOnly(2010, 4, 15), "contact@techsphere.com");
            Organisation greenFuture = new Organisation("GreenFuture Initiatives", "A non-profit organization dedicated to promoting sustainability and renewable energy projects.", new DateOnly(2005, 8, 23), "info@greenfuture.org");
            Organisation culturalHorizons = new Organisation("Cultural Horizons", "Fostering cultural exchange and creative arts through events and workshops.", new DateOnly(2018, 3, 9), "hello@culturalhorizons.com");
            Organisation healthLink = new Organisation("HealthLink International", "Connecting healthcare providers with innovative medical technologies and solutions worldwide.", new DateOnly(2012, 11, 5), "support@healthlinkint.com");
            Organisation futureTech = new Organisation("FutureTech Solutions", "Innovative solutions provider specializing in robotics and automation for industries.", new DateOnly(2015, 6, 20), "info@futuretech.com");

            // Create tickets
            Ticket studayT856 = new Ticket { Event = studay, Visitor = stan, PurchaseDate = new DateTime(2024, 06, 22, 15, 22, 36), PaymentMethode = PaymentMethode.Online };
            Ticket studayT711 = new Ticket { Event = studay, Visitor = emma, PurchaseDate = new DateTime(2024, 09, 18, 16, 32, 10), PaymentMethode = PaymentMethode.AtTheDoor };
            Ticket techExpoT522 = new Ticket { Event = techExpo, Visitor = lucas, PurchaseDate = new DateTime(2024, 11, 05, 10, 12, 55), PaymentMethode = PaymentMethode.AtTheDoor };
            Ticket musicFestT333 = new Ticket { Event = musicFest, Visitor = emma, PurchaseDate = new DateTime(2024, 1, 19, 8, 8, 10), PaymentMethode = PaymentMethode.Online };
            Ticket businessSummitT122 = new Ticket { Event = businessSummit, Visitor = emma, PurchaseDate = new DateTime(2024, 10, 9, 18, 28, 15), PaymentMethode = PaymentMethode.Online };

            // Link organisations to events
            studay.Organisation = techSphere;
            techExpo.Organisation = greenFuture;
            businessSummit.Organisation = culturalHorizons;
            musicFest.Organisation = healthLink;

            // Link events to IdentityUsers
            var user1 = await _userManager.FindByEmailAsync("user1@example.com");
            var user2 = await _userManager.FindByEmailAsync("user2@example.com");
            var user3 = await _userManager.FindByEmailAsync("user3@example.com");
            var user4 = await _userManager.FindByEmailAsync("user4@example.com");
            var user5 = await _userManager.FindByEmailAsync("user5@example.com");

            studay.UserId = user1?.Id;
            techExpo.UserId = user2?.Id;
            businessSummit.UserId = user3?.Id;
            musicFest.UserId = user4?.Id;

            // Add entities to the context
            _context.Events.AddRange(studay, techExpo, businessSummit, musicFest);
            _context.Visitors.AddRange(stan, emma, lucas, olivia);
            _context.Organisations.AddRange(techSphere, greenFuture, culturalHorizons, healthLink, futureTech);
            _context.Tickets.AddRange(studayT856, studayT711, techExpoT522, musicFestT333, businessSummitT122);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Clear the change tracker
            _context.ChangeTracker.Clear();
        }
    }
}
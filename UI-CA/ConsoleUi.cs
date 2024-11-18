using EM.UI.CA.Extentions;

namespace UI;

public class ConsoleUi
{
   private List<Event> events { get; set; }
   private List<Visitor> visitors { get; set; }
   
    public void Seed()
    {
        events=new List<Event>();
        visitors = new List<Visitor>();
        
        var studay = new Event()
        {
            EventId = 1,
            EventName = "Studay",
            EventDate = new DateTime(2024, 09, 18, 14, 00, 00),
            TicketPrice = null,
            EventDescription = "Party for students",
            Category = EventCategory.Festival
            
        };
        events.Add(studay);
        
        var techExpo = new Event()
        {
            EventId = 2,
            EventName = "Tech Expo",
            EventDate = new DateTime(2024, 11, 05, 10, 00, 00),
            TicketPrice = 25.00m,
            EventDescription = "A showcase of the latest in technology",
            Category = EventCategory.Conference
        };
        events.Add(techExpo);
        
        var businessSummit = new Event()
        {
            EventId = 3,
            EventName = "Business Summit",
            EventDate = new DateTime(2024, 10, 10, 09, 00, 00),
            TicketPrice = 100.00m,
            EventDescription = "Summit for business networking and learning",
            Category = EventCategory.Networking
        };
        events.Add(businessSummit);

        var musicFest = new Event()
        {
            EventId = 4,
            EventName = "Music Fest",
            EventDate = new DateTime(2024, 08, 25, 16, 00, 00),
            TicketPrice = 50.00m,
            EventDescription = "A weekend of music and fun",
            Category = EventCategory.Music
        };
        events.Add(musicFest);


        var stanSchins = new Visitor()
        {
            VisitorId = 1,
            FirstName = "Stan",
            LastName = "Schins",
            Email = "stanschins@gmail.com",
            PhoneNumber = "0484710770",
            City = "Antwerpen"
        };
        visitors.Add(stanSchins);
        
        var emmaJones = new Visitor()
        {
            VisitorId = 2,
            FirstName = "Emma",
            LastName = "Jones",
            Email = "emma.jones@example.com",
            PhoneNumber = "0475123456",
            City = "Gent"
        };
        visitors.Add(emmaJones);

        var lucasBaker = new Visitor()
        {
            VisitorId = 3,
            FirstName = "Lucas",
            LastName = "Baker",
            Email = "lucas.baker@example.com",
            PhoneNumber = "0476987654",
            City = "Brussel"
        };
        visitors.Add(lucasBaker);

        var oliviaSmith = new Visitor()
        {
            VisitorId = 4,
            FirstName = "Olivia",
            LastName = "Smith",
            Email = "olivia.smith@example.com",
            PhoneNumber = "0476112233",
            City = "Antwerpen"
        };
        visitors.Add(oliviaSmith);

        studay.Visitors.Add(stanSchins);
        studay.Visitors.Add(emmaJones);
        
        techExpo.Visitors.Add(stanSchins);
        techExpo.Visitors.Add(lucasBaker);
        
        businessSummit.Visitors.Add(oliviaSmith);
        businessSummit.Visitors.Add(emmaJones);
        
        musicFest.Visitors.Add(oliviaSmith);
        musicFest.Visitors.Add(lucasBaker);
        

    }
    public void Run()
    {
        Seed();

        bool running = true;
        while (running)
        {
            ShowMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "0":
                    running = false;
                    break;
                case "1":
                    ShowAllEvents();
                    break;
                case "2":
                    ShowEventByCategory();
                    break;
                case "3":
                    ShowAllVisitors();
                    break;
                case "4":
                    ShowVisitorsByNameOrCity();
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }

       
    
    }
    private void ShowMenu()
    {
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("============================");
        Console.WriteLine("0) Quit");
        Console.WriteLine("1) Show all events");
        Console.WriteLine("2) Show all events by category");
        Console.WriteLine("3) Show all visitors");
        Console.WriteLine("4) Show all visitors by name and/or city");
        Console.Write("Choice (0-4): ");
    }

    private void ShowAllEvents()
    {
        Console.WriteLine("\nAll events:");
        Console.WriteLine("=========================");
        foreach (var evnt in events) //event kan niet worden gebruikt als var omdat het een gereserveerd woord is
        {
            Console.WriteLine(evnt.ToStringEvent());
        }
    }
    
    private void ShowEventByCategory()
    {
        // Dynamisch weergeven van categorieën
        Console.WriteLine("\nCategories:");
        var categories = Enum.GetValues(typeof(EventCategory)).Cast<EventCategory>().ToList();
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {categories[i]}");
        }

        Console.WriteLine("\nEnter category (choose number): ");
        if (int.TryParse(Console.ReadLine(), out int categoryIndex) && categoryIndex >= 1 && categoryIndex <= categories.Count)
        {
            var selectedCategory = categories[categoryIndex - 1];
            var eventsInCategory = events.Where(e => e.Category == selectedCategory).ToList();

            Console.WriteLine($"\nEvents in this category: {selectedCategory}");
            Console.WriteLine("=========================");

            if (eventsInCategory.Any())
            {
                foreach (var evnt in eventsInCategory)
                {
                    Console.WriteLine(evnt.ToStringEvent());
                }
            }
            else
            {
                Console.WriteLine("No events available in this category.");
            }
        }
        else
        {
            Console.WriteLine("Invalid category, please try again.");
        }
    }



    private void ShowAllVisitors()
    {
        Console.WriteLine("\nAll visitors:");
        Console.WriteLine("=========================");
        foreach (var visitor in visitors)
        {
            Console.WriteLine(visitor.ToStringVisitor());
        }
    }
    
    private void ShowVisitorsByNameOrCity()
    {
        Console.WriteLine("\nEnter a first name or leave blank:");
        string firstNameInput = Console.ReadLine()?.ToLower();

        Console.WriteLine("\nEnter a city or leave blank:");
        string cityInput = Console.ReadLine()?.ToLower();

        Console.WriteLine("\nVisitors with matching search criteria:");
        Console.WriteLine("=========================");

        foreach (var visitor in visitors)
        {
            bool matchesFirstName = string.IsNullOrEmpty(firstNameInput) || visitor.FirstName.ToLower().Contains(firstNameInput);
            bool matchesCity = string.IsNullOrEmpty(cityInput) || (visitor.City != null && visitor.City.ToLower().Contains(cityInput));

            if (matchesFirstName && matchesCity)
            {
                Console.WriteLine(visitor.ToStringVisitor());
            }
            else
            {
                Console.WriteLine("Invalid criteria, please try again.");
                break;
            }
        }
    }

}
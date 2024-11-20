using BL;
using EM.DAL;
using EM.UI.CA.Extentions;

namespace UI;

public class ConsoleUi
{
    private readonly IManager _manager;

    //losse koppeling van pres layer naar BL
    public ConsoleUi(IManager manager)
    {
        _manager = manager;
    }
  
    public void Run()
    {
        InMemoryRepository.Seed();
        
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
        foreach (var evnt in _manager.GetAllEvents()) //event kan niet worden gebruikt als var omdat het een gereserveerd woord is
        {
            Console.WriteLine(evnt.ToStringEvent());
        }
    }
    
    private void ShowEventByCategory()
    {
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
            var eventsInCategory = _manager.GetEventsByCategory(selectedCategory);

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
        foreach (var visitor in _manager.GetAllVisitors())
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

        var visitors = _manager.GetVisitorsByNameOrCity(firstNameInput, cityInput);
        if (visitors.Any())
        {
            foreach (var visitor in visitors)
            {
                Console.WriteLine(visitor.ToStringVisitor());
            }
        }
        else
        {
            Console.WriteLine("No visitors match the given criteria.");
        }
    }


}
using System.ComponentModel.DataAnnotations;
using EM.BL;
using EM.BL.Domain;
using EM.UI.CA.Extentions;

namespace EM.UI.CA;

public class ConsoleUi
{
    private readonly IManager _manager;

    //losse koppeling van pres layer naar BL
    public ConsoleUi(IManager manager )
    {
        _manager = manager;
    }
  
    public void Run()
    {
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
                case "5":
                    AddNewEvent();
                    break;
                case "6":
                    AddNewVisitor();
                    break;
                case "7":
                    CreateTicket();
                    break;
                case "8":
                    DeleteTicket();
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
        Console.WriteLine("5) Add a new event");
        Console.WriteLine("6) Add a new visitor");
        Console.WriteLine("7) Add event to visitor");
        Console.WriteLine("8) Remove event from visitor");
        Console.Write("Choice (0-8): ");
    }


    private void ShowAllEvents()
    {
        Console.WriteLine("\nAll events:");
        Console.WriteLine("=========================");
        foreach (var evnt in _manager.GetAllEventsWithOrganisation()) 
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
            List<Event> eventsInCategory = _manager.GetEventsByCategory(selectedCategory).ToList();

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
        foreach (var visitor in _manager.GetAllVisitorsWithEvents())
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

        List<Visitor> visitors = _manager.GetVisitorsByNameOrCity(firstNameInput, cityInput).ToList();
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

  private void AddNewEvent()
{
    try
    {
        Console.WriteLine("\nAdd a new Event");
        Console.WriteLine("============================");

        Console.Write("Event Name: ");
        string name = Console.ReadLine();

        Console.Write("Event Description: ");
        string description = Console.ReadLine();

        Console.Write("Event Date (yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            throw new FormatException("Invalid date format.");
        }

        Console.Write("Ticket Price (leave empty for free): ");
        string ticketPriceInput = Console.ReadLine();
        decimal? ticketPrice = string.IsNullOrEmpty(ticketPriceInput)
            ? null
            : Convert.ToDecimal(ticketPriceInput);

        Console.WriteLine("Select Event Category:");
        var categories = Enum.GetValues(typeof(EventCategory)).Cast<EventCategory>().ToList();
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {categories[i]}");
        }

        Console.Write("Category (choose number): ");
        if (!int.TryParse(Console.ReadLine(), out int categoryIndex) || categoryIndex < 1 || categoryIndex > categories.Count)
        {
            throw new ArgumentException("Invalid category selection.");
        }
        EventCategory category = categories[categoryIndex - 1];

        // Voeg het nieuwe event toe via de manager
        var newEvent = _manager.AddEvent(name, date, ticketPrice, description, category);
        Console.WriteLine($"\nEvent successfully added: {newEvent.EventName}");
    }
    catch (ValidationException ex)
    {
        // Splits de foutmeldingen en toon ze met \n
        var errorMessages = ex.Message.Split('|');
        Console.WriteLine("Validation errors:");
        foreach (var error in errorMessages)
        {
            Console.WriteLine($"- {error.Trim()}");
        }
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"Input error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
    finally
    {
        // Terugkeren naar het menu
        Console.WriteLine("Returning to the main menu...");
    }
}

private void AddNewVisitor()
{
    try
    {
        Console.WriteLine("\nAdd a new Visitor");
        Console.WriteLine("============================");

        Console.Write("First Name: ");
        string firstName = Console.ReadLine();

        Console.Write("Last Name: ");
        string lastName = Console.ReadLine();

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Phone Number: ");
        string phoneNumber = Console.ReadLine();

        Console.Write("City: ");
        string city = Console.ReadLine();

        // Voeg de nieuwe visitor toe via de manager
        var newVisitor = _manager.AddVisitor(firstName, lastName, email, phoneNumber, city);
        Console.WriteLine($"\nVisitor successfully added: {newVisitor.FirstName} {newVisitor.LastName}");
    }
    catch (ValidationException ex)
    {
        // Splits de foutmeldingen en toon ze met \n
        var errorMessages = ex.Message.Split('|');
        Console.WriteLine("Validation errors:");
        foreach (var error in errorMessages)
        {
            Console.WriteLine($"- {error.Trim()}");
        }
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"Input error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
    finally
    {
        // Terugkeren naar het menu
        Console.WriteLine("Returning to the main menu...");
    }
}

private void CreateTicket()
{
    try
    {
        Console.WriteLine("\nAdd an event to a visitor");
        Console.WriteLine("============================");

        //kies visitor
        Console.WriteLine("\nSelect a visitor:");
        List<Visitor> visitors = _manager.GetAllVisitors().ToList();
        for (int i = 0; i < visitors.Count(); i++)
        {
            var visitor = visitors.ElementAt(i);
            Console.WriteLine($"{i + 1}) {visitor.FirstName} {visitor.LastName}");
        }
        Console.Write("Visitor (choose number): ");
        if (!int.TryParse(Console.ReadLine(), out int visitorIndex) || visitorIndex < 1 || visitorIndex > visitors.Count())
        {
            throw new ArgumentException("Invalid visitor selection.");

        }
        var selectedVisitor = visitors.ElementAt(visitorIndex - 1);

        //kies event
        Console.WriteLine("\nSelect an event:");
        List<Event> events = _manager.GetAllEvents().ToList();
        for (int i = 0; i < events.Count(); i++)
        {
            var evnt = events.ElementAt(i);
            Console.WriteLine($"{i + 1}) {evnt.EventName} ({evnt.EventDate:yyyy-MM-dd HH:mm})");

        }
        Console.Write("Event (choose number): ");
        if (!int.TryParse(Console.ReadLine(), out int eventIndex) || eventIndex < 1 || eventIndex > events.Count())
        {
            throw new ArgumentException("Invalid event selection.");
        }
        var selectedEvent = events.ElementAt(eventIndex - 1);
        
        // Kies PaymentMethod
        Console.WriteLine("\nSelect payment method:");
        var methodes = Enum.GetValues(typeof(PaymentMethode)).Cast<PaymentMethode>().ToList();
        for (int i = 0; i < methodes.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {methodes[i]}");
        }
        Console.Write("Purchase method (choose number): ");
        if (!int.TryParse(Console.ReadLine(), out int paymentMethodIndex) || (paymentMethodIndex > methodes.Count))
        {
            throw new ArgumentException("Invalid payment method selection.");
        }
        var selectedMethod = methodes[paymentMethodIndex - 1];


        _manager.AddTicket(selectedEvent.EventId, selectedVisitor.VisitorId, selectedMethod);
        Console.WriteLine($"\nTicket successfully created for {selectedVisitor.FirstName} {selectedVisitor.LastName} to {selectedEvent.EventName}");
    }
    catch (ValidationException ex)
    {
        // Splits de foutmeldingen en toon ze met \n
        var errorMessages = ex.Message.Split('|');
        Console.WriteLine("Validation errors:");
        foreach (var error in errorMessages)
        {
            Console.WriteLine($"- {error.Trim()}");
        }
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"Input error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
    finally
    {
        // Terugkeren naar het menu
        Console.WriteLine("Returning to the main menu...");
    }
}

public void DeleteTicket()
{
    try
    {
        Console.WriteLine("\nRemove a ticket");
        Console.WriteLine("============================");

        // Kies een Visitor
        Console.WriteLine("\nSelect a visitor:");
        List<Visitor> visitors = _manager.GetAllVisitors().ToList();
        for (int i = 0; i < visitors.Count(); i++)
        {
            var visitor = visitors.ElementAt(i);
            Console.WriteLine($"{i + 1}) {visitor.FirstName} {visitor.LastName}");
        }
        
        Console.Write("Visitor (choose number): ");
        if (!int.TryParse(Console.ReadLine(), out int visitorIndex) || visitorIndex < 1 || visitorIndex > visitors.Count())
        {
            throw new ArgumentException("Invalid visitor selection.");
        }
        var selectedVisitor = visitors.ElementAt(visitorIndex - 1);
        
        // Kies een Event van de geselecteerde Visitor
        Console.WriteLine("\nSelect an event to remove:");
        List<Event> events = _manager.GetEventsByVisitor(selectedVisitor.VisitorId).ToList();
        if (!events.Any())
        {
            Console.WriteLine("This visitor has no events to remove.");
            return;
        }

        for (int i = 0; i < events.Count(); i++)
        {
            var evnt = events.ElementAt(i);
            Console.WriteLine($"{i + 1}) {evnt.EventName} ({evnt.EventDate:yyyy-MM-dd HH:mm})");
        }
        
        Console.Write("Event (choose number): ");
        if (!int.TryParse(Console.ReadLine(), out int eventIndex) || eventIndex < 1 || eventIndex > events.Count())
        {
            throw new ArgumentException("Invalid event selection.");
        }
        var selectedEvent = events.ElementAt(eventIndex - 1);

        // Verwijder het ticket via de manager
        _manager.RemoveTicket(selectedEvent.EventId, selectedVisitor.VisitorId);
        Console.WriteLine($"\nTicket successfully removed for {selectedVisitor.FirstName} {selectedVisitor.LastName} from {selectedEvent.EventName}");
    }
    catch (ValidationException ex)
    {
        // Splits de foutmeldingen en toon ze met \n
        var errorMessages = ex.Message.Split('|');
        Console.WriteLine("Validation errors:");
        foreach (var error in errorMessages)
        {
            Console.WriteLine($"- {error.Trim()}");
        }
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"Input error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
    finally
    {
        // Terugkeren naar het menu
        Console.WriteLine("Returning to the main menu...");
    }
}






}
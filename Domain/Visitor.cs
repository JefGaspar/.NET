namespace UI;

public class Visitor
{
    public int VisitorId { get; set; } //property
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string City { get; set; }

    //kan ICollection zijn ipv list omdat het voor een veel relatie
    //List geeft een harde koppeling omdat het uitgangspunt index based is wat niet nodig is voor een nav property
    public ICollection<Event> Events { get; set; } = new List<Event>();

    public Visitor(){}
    
    public Visitor(int visitorId, string firstName, string lastName, string email, string phoneNumber, string city, List<Event> events)
    {
        this.VisitorId = visitorId;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.PhoneNumber = phoneNumber;
        this.City = city;
        this.Events = events;
    }
    
    
}
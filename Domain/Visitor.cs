namespace UI;

public class Visitor
{
    public int VisitorId { get; set; } //property
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string City { get; set; }

    public List<Event> Events { get; set; }

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
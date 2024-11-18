namespace UI;

public class Organisation
{
    public int OrgId { get; set; }
    public string OrgName { get; set; }
    public string OrgDescription { get; set; }
    public DateTime FoundedDate { get; set; }
    public string ContactEmail { get; set; }
    
    // Relatie met events (een organisatie kan meerdere events organiseren)
    public List<Event> Events { get; set; }

}
using System.ComponentModel.DataAnnotations;

namespace UI;

public class Organisation
{
    [Key]
    public int OrgId { get; set; }
    public string OrgName { get; set; }
    public string OrgDescription { get; set; }
    public DateTime FoundedDate { get; set; }
    public string ContactEmail { get; set; }
    
    // Relatie met events (een organisatie kan meerdere events organiseren)
    public ICollection<Event> Events { get; set; } = new List<Event>();

}
using System.ComponentModel.DataAnnotations;

namespace EM.BL.Domain;

public class Organisation
{
    [Key]
    public int OrgId { get; set; }
    public string OrgName { get; set; }
    public string OrgDescription { get; set; }
    public DateOnly FoundedDate { get; set; }
    public string ContactEmail { get; set; }
    
    // Relatie met events (een organisatie kan meerdere events organiseren)
    public ICollection<Event> Events { get; set; }

    public Organisation() { }

    public Organisation(string orgName, string orgDescription, DateOnly foundedDate, string contactEmail)
    {
        this.OrgName = orgName;
        this.OrgDescription = orgDescription;
        this.FoundedDate = foundedDate;
        this.ContactEmail = contactEmail;
    }
}
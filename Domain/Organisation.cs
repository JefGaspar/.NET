using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UI;

public class Organisation
{
    [Key]
    public int OrgId { get; set; }
    public string OrgName { get; set; }
    public string OrgDescription { get; set; }
    public DateOnly FoundedDate { get; set; }
    public string ContactEmail { get; set; }
    [NotMapped] 
    // Relatie met events (een organisatie kan meerdere events organiseren)
    public ICollection<Event> Events { get; set; } = new List<Event>();

    public Organisation(int orgId, string orgName, string orgDescription, DateOnly foundedDate, string contactEmail)
    {
        this.OrgId = orgId;
        this.OrgName = orgName;
        this.OrgDescription = orgDescription;
        this.FoundedDate = FoundedDate;
        this.ContactEmail = contactEmail;
    }
}
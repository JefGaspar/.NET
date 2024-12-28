using System.ComponentModel.DataAnnotations;

namespace EM.BL.Domain;

public class Ticket
{
    [Required]
    [Key]
    public Event Event { get; set; }
    [Required]
    [Key]
    public Visitor Visitor { get; set; }

    public DateTime PurchaseDate { get; set; }
    public PurchaseMethode PurchaseMethode { get; set; }
    
    public Ticket() {}

    public Ticket(Event evnt, Visitor visitor, DateTime purchaseDate, PurchaseMethode purchaseMethode)
    {
        this.Event = evnt;
        this.Visitor = visitor;
        this.PurchaseDate = purchaseDate;
        this.PurchaseMethode = purchaseMethode;
    }
}
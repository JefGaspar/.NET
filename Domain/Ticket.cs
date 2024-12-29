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
    public PaymentMethode PaymentMethode { get; set; }
    
    public Ticket() {}

    public Ticket(Event evnt, Visitor visitor, DateTime purchaseDate, PaymentMethode paymentMethode)
    {
        this.Event = evnt;
        this.Visitor = visitor;
        this.PurchaseDate = purchaseDate;
        this.PaymentMethode = paymentMethode;
    }
}
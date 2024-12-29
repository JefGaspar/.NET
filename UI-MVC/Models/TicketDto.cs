using EM.BL.Domain;

public class TicketDto
{
    public int VisitorId { get; set; }
    public int EventId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public PaymentMethode PaymentMethod { get; set; } // Online, AtTheDoor
}

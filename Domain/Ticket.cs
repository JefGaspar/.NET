using System.ComponentModel.DataAnnotations;

namespace UI;

public class Ticket
{
    [Required]
    [Key]
    public Event Event { get; set; }
    [Required]
    [Key]
    public Visitor Visitor { get; set; }

    public DateTime PurchaseDate { get; set; }
}
using EM.BL.Domain;

namespace EM.UI.MVC.Models
{
    public class TicketViewModel
    {
        public int VisitorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public PaymentMethode PaymentMethode { get; set; }
    }
}
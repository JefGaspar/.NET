using EM.BL.Domain;

namespace EM.BL
{
    public interface IManager
    {
        //IEnumerable ipv list omdat u BL niet index based waarden moet doorgeven aan UI, isoleren van archtectuur
        public IEnumerable<Event> GetAllEvents();
        Event GetEvent(int id);
        public Event GetEventWithVisitors(int id);
        public IEnumerable<Event> GetEventsByCategory(EventCategory category);
        public Event AddEvent(string name, DateTime date, decimal? ticketPrice, string description, EventCategory category, string userId);
        void ChangeEvent(Event evnt); // Renamed from ChangeEvent
        public Visitor GetVisitor(int id);
        public IEnumerable<Visitor> GetAllVisitors();
        public IEnumerable<Visitor> GetVisitorsByNameOrCity(string firstName, string city);
        public IEnumerable<Event> GetEventsByVisitor(int visitorId);
        public Visitor AddVisitor(string firstName, string lastName, string email, string phoneNumber, string city);
        public IEnumerable<Event> GetAllEventsWithOrganisation();
        public IEnumerable<Visitor> GetAllVisitorsWithEvents(); 
        public Ticket AddTicket(int eventId, int visitorId, PaymentMethode paymentMethode);
        public void RemoveTicket(int eventId, int visitorId);
        public IEnumerable<Event> GetAvailableEventsForVisitor(int visitorId);
        IEnumerable<Organisation> GetAllOrganisations();
        public Organisation AddOrganisation(string orgName, string orgDescription, DateOnly foundedDate, string contactEmail);
    } 
    
}

//3 niveau's waarop we interfaces gebruiken in onze architectuur
// ICollection -> domein modellen de nav properties voor veelzijdes 
// IEnumareble -> de return type van verzaleming van data voor managers en repos 
// eigen interfaces -> voor losse koppeling tussen lagen
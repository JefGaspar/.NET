using UI;

namespace BL
{
    public interface IManager
    {
        public Event GetEvent(int id);
        //IEnumerable ipv list omdat u BL niet index based waarden moet doorgeven aan UI, isoleren van archtectuur
        public IEnumerable<Event> GetAllEvents();
        public IEnumerable<Event> GetEventsByCategory(EventCategory category);
        public Event AddEvent(string name, DateTime date, decimal? ticketPrice, string description, EventCategory category);
        public Visitor GetVisitor(int id);
        public IEnumerable<Visitor> GetAllVisitors();
        public IEnumerable<Visitor> GetVisitorsByNameOrCity(string firstName, string city);
        public Visitor AddVisitor(string firstName, string lastName, string email, string phoneNumber, string city);
        public Organisation AddOrganisation(int orgId, string orgName, string orgDescription, DateOnly foundedDate, string contactEmai);
        public IEnumerable<Event> GetAllEventsWithOrganisation();
        public IEnumerable<Visitor> GetAllVisitorsWithEvents();
    } 
    
}

//3 niveau's waarop we interfaces gebruiken in onze architectuur
// ICollection -> domein modellen de nav properties voor veelzijdes 
// IEnumareble -> de return type van verzaleming van data voor managers en repos 
// eigen interfaces -> voor losse koppeling tussen lagen
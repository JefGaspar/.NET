using EM.DAL;
using UI; 

namespace BL
{
    public class Manager : IManager
    {
        private readonly IRepository _repository;

        //lage koppeling van BL naar DAL
        public Manager(IRepository repository)
        {
            _repository = repository;
        }

        public Event GetEvent(int id)
        {
            return _repository.ReadEvent(id);
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _repository.ReadAllEvents();
        }

        public IEnumerable<Event> GetEventsByCategory(EventCategory category)
        {
            return _repository.ReadEventsByCategory(category);
        }

        public Event AddEvent(string name, DateTime date, decimal? ticketPrice, string description, EventCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Event name cannot be empty.");
            if (date < DateTime.Now)
                throw new ArgumentException("Event date cannot be in the past.");
            
            var newEvent = new Event
            {
                EventName = name,
                EventDate = date,
                TicketPrice = ticketPrice,
                EventDescription = description,
                Category = category
            };

            _repository.CreateEvent(newEvent);
            return newEvent;
        }

        public Visitor GetVisitor(int id)
        {
            return _repository.ReadVisitor(id);
        }

        public IEnumerable<Visitor> GetAllVisitors()
        {
            return _repository.ReadAllVisitors();
        }

        public IEnumerable<Visitor> GetVisitorsByNameOrCity(string firstName, string city)
        {
            return _repository.ReadVisitorsByNameOrCity(firstName, city);
        }

        public Visitor AddVisitor(string firstName, string lastName, string email, string phoneNumber, string city)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Email is invalid.");
            
            var newVisitor = new Visitor
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                City = city
            };

            _repository.CreateVisitor(newVisitor);
            return newVisitor;
        }
    }
}

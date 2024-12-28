using System.ComponentModel.DataAnnotations;
using EM.BL.Domain;
using EM.DAL;

namespace EM.BL
{
    public class Manager : IManager
    {
        private readonly IRepository _repository;

        //lage koppeling van BL naar DAL
        public Manager(IRepository repository)
        {
            _repository = repository;
        }
        
        public IEnumerable<Event> GetAllEvents()
        {
            return _repository.ReadAllEvents();
        }

        public Event GetEvent(int id)
        {
            return _repository.ReadEvent(id);
        }

        public Event GetEventWithVisitors(int id)
        {
            return _repository.ReadEventWithVisitors(id);
        }
        
        public IEnumerable<Event> GetEventsByCategory(EventCategory category)
        {
            return _repository.ReadEventsByCategory(category);
        }

        public Event AddEvent(string name, DateTime date, decimal? ticketPrice, string description, EventCategory category)
        {
            // Maak het nieuwe Event object aan
            var newEvent = new Event
            {
                EventName = name,
                EventDate = date,
                TicketPrice = ticketPrice,
                EventDescription = description,
                Category = category
            };

            // Valideer het nieuwe object
            ValidateObject(newEvent);

            // Voeg het gevalideerde object toe via de repository
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
            // Maak het nieuwe Visitor object aan
            var newVisitor = new Visitor
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                City = city
            };

            // Valideer het nieuwe object
            ValidateObject(newVisitor);

            // Voeg het gevalideerde object toe via de repository
            _repository.CreateVisitor(newVisitor);
            return newVisitor;
        }
        
        public IEnumerable<Event> GetAllEventsWithOrganisation()
        {
            return _repository.ReadAllEventsWithOrganisation();
        }

        public IEnumerable<Visitor> GetAllVisitorsWithEvents()
        {
            return _repository.ReadAllVisitorsWithEvents();
        }

        public Ticket AddTicket(Event evnt, Visitor visitor, DateTime purchaseDate, PurchaseMethode purchaseMethode)
        {
            if (_repository.GetTicket(evnt.EventId, visitor.VisitorId) != null)
            {
                throw new ValidationException("A ticket for this visitor and event already exists.");
            }

            var newTicket = new Ticket
            {
                Event = evnt,
                Visitor = visitor,
                PurchaseDate = purchaseDate,
                PurchaseMethode = purchaseMethode
            };
            
            ValidateObject(newTicket);
            
            _repository.CreateTicket(newTicket);
            return newTicket;
        }

        public void RemoveTicket(int eventId, int visitorId)
        {
            _repository.DeleteTicket(eventId, visitorId);
        }

        public IEnumerable<Event> GetEventsOfVisitor(int visitorId)
        {
            return _repository.ReadEventsOfVisitor(visitorId);
        }

        private void ValidateObject(object obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj);

            // Valideer standaard attributen
            //bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, validateAllProperties: true);

            // Controleer of het object IValidatableObject implementeert
            if (obj is IValidatableObject validatable)
            {
                var customValidationResults = validatable.Validate(validationContext);
                validationResults.AddRange(customValidationResults);
            }

            if (validationResults.Any())
            {
                var errors = string.Join("|", validationResults.Select(vr => vr.ErrorMessage));
                throw new ValidationException($"{errors}");
            }
        }

    }
    
}

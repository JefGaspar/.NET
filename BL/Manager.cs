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

        public Event AddEvent(string name, DateTime date, decimal? ticketPrice, string description, EventCategory category, string userId)
        {
            // Maak het nieuwe Event object aan
            var newEvent = new Event
            {
                EventName = name,
                EventDate = date,
                TicketPrice = ticketPrice,
                EventDescription = description,
                Category = category,
                UserId = userId
            };

            // Valideer het nieuwe object
            ValidateObject(newEvent);

            // Voeg het gevalideerde object toe via de repository
            _repository.CreateEvent(newEvent);
            return newEvent;
        }
        
        public void ChangeEvent(Event evnt) // Renamed from ChangeEvent
        {
            ValidateObject(evnt);
            _repository.UpdateEvent(evnt);
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
        public IEnumerable<Event> GetEventsByVisitor(int visitorId)
        {
            return _repository.ReadEventsByVisitor(visitorId);
        }
        
        public Ticket AddTicket(int eventId, int visitorId, PaymentMethode paymentMethode)
        {
            var evnt = _repository.ReadEvent(eventId) ?? throw new Exception("Event not found.");
            var visitor = _repository.ReadVisitor(visitorId) ?? throw new Exception("Visitor not found.");

            if (_repository.ReadTicket(eventId, visitorId) != null)
            {
                throw new ValidationException("A ticket for this visitor and event already exists.");
            }

            var newTicket = new Ticket
            {
                Event = evnt,
                Visitor = visitor,
                PurchaseDate = DateTime.Now,
                PaymentMethode = paymentMethode
            };

            ValidateObject(newTicket);
            _repository.CreateTicket(newTicket);
            return newTicket;
        }

        public void RemoveTicket(int eventId, int visitorId)
        {
            _repository.DeleteTicket(eventId, visitorId);
        }

       
        public IEnumerable<Event> GetAvailableEventsForVisitor(int visitorId)
        {
            var allEvents = _repository.ReadAllEvents();
            var visitorEvents = _repository.ReadEventsByVisitor(visitorId);
            return allEvents.Except(visitorEvents);
        }

        public IEnumerable<Organisation> GetAllOrganisations()
        {
            return _repository.ReadAllOrganisations();
        }

        public Organisation AddOrganisation(string orgName, string orgDescription, DateOnly foundedDate, string contactEmail)
        {
            var newOrganisation = new Organisation
            {
                OrgName = orgName,
                OrgDescription = orgDescription,
                FoundedDate = foundedDate,
                ContactEmail = contactEmail
            };

            _repository.CreateOrganisation(newOrganisation);
            return newOrganisation;
        }


        private void ValidateObject(object obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj);

            // Valideer standaard attributen
            Validator.TryValidateObject(obj, validationContext, validationResults, validateAllProperties: true);

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

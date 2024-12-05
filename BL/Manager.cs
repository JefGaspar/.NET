using System.ComponentModel.DataAnnotations;
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

        private void ValidateObject(object obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj);

            // Valideer standaard attributen
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, validateAllProperties: true);

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

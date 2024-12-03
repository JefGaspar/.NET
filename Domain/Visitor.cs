using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UI
{
    public class Visitor : IValidatableObject
    {
        public int VisitorId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();

        public Visitor() { }

        public Visitor(int visitorId, string firstName, string lastName, string email, string phoneNumber, string city, List<Event> events)
        {
            this.VisitorId = visitorId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.City = city;
            this.Events = events;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Custom validation: ensure that FirstName and LastName are not identical
            if (FirstName != null && LastName != null && FirstName.Equals(LastName, StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("First name and last name cannot be the same.", new[] { nameof(FirstName), nameof(LastName) });
            }

            // Custom validation: ensure that City is not null or empty if Events are present
            if (Events.Count > 0 && string.IsNullOrWhiteSpace(City))
            {
                yield return new ValidationResult("City is required if the visitor is attending events.", new[] { nameof(City) });
            }
        }
    }
}

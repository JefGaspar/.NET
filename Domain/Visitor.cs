using System.ComponentModel.DataAnnotations;

namespace EM.BL.Domain
{
    public class Visitor : IValidatableObject
    {
        [Key] public int VisitorId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
        public Visitor() { }

        public Visitor(string firstName, string lastName, string email, string phoneNumber, string city)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.City = city;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Lijst om validatiefouten op te slaan
            List<ValidationResult> errors = new List<ValidationResult>();

            // Controleer of FirstName en LastName identiek zijn
            if (FirstName != null && LastName != null && FirstName.Equals(LastName, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new ValidationResult(
                    "First name and last name cannot be the same.",
                    new[] { nameof(FirstName), nameof(LastName) }
                ));
            }
            
            // Retourneer alle validatiefouten
            return errors;
        }

    }
}

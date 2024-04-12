using System.ComponentModel.DataAnnotations;

namespace Practice21.MinimalAPI.Models
{
    public class PhoneBookEntry : IEquatable<PhoneBookEntry>
    {
        public int Id { get; set; } = 0;

        [Display(Name = "Фамилия"), StringLength(60, MinimumLength = 2), Required]
        public string? LastName { get; set; } = string.Empty;

        [Display(Name = "Имя"), StringLength(60, MinimumLength = 2), Required]
        public string? FirstName { get; set; } = string.Empty;

        [Display(Name = "Отчество"), StringLength(60, MinimumLength = 2), Required]
        public string? MiddleName { get; set; } = string.Empty;

        [Display(Name = "Номер телефона"), DisplayFormat(DataFormatString = "{0:+0 (000) 000-00-00}")]
        public long PhoneNumber { get; set; } = 0;

        [Display(Name = "Адрес"), StringLength(60, MinimumLength = 2), Required]
        public string? Address { get; set; } = string.Empty;

        [Display(Name = "Описание"), StringLength(100)]
        public string? Description { get; set; } = string.Empty;

        public string? Name { get { return $"{LastName} {FirstName} {MiddleName}"; } }

        public PhoneBookEntry(){}

        public PhoneBookEntry(PhoneBookEntry? phoneBookEntry)
        {
            if (phoneBookEntry != null)
            {
                Id = phoneBookEntry.Id;
                LastName = phoneBookEntry.LastName;
                FirstName = phoneBookEntry.FirstName;
                MiddleName = phoneBookEntry.MiddleName;
                PhoneNumber = phoneBookEntry.PhoneNumber;
                Address = phoneBookEntry.Address;
                Description = phoneBookEntry.Description;
            }
        }

        public bool Equals(PhoneBookEntry? other)
        {
            if (other == null) return false;
            return Id == other.Id &&
                LastName == other.LastName &&
                FirstName == other.FirstName &&
                MiddleName == other.MiddleName &&
                PhoneNumber == other.PhoneNumber &&
                Address == other.Address &&
                Description == other.Description;
        }
    }
}

using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
  public class PersonUpdateRequest
  {
    [Required(ErrorMessage = "PersonID cannot be blank")]
    public Guid PersonID { get; set; }

    [Required(ErrorMessage = "Name cannot be blank")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email cannot be blank")]
    [EmailAddress(ErrorMessage = "Must be a valid email address")]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public GenderOptions Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    public Person ToPerson()
    {
      return new Person { PersonID = PersonID, PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), CountryID = CountryID, Address = Address, ReceiveNewsLetters = ReceiveNewsLetters };
    }
  }
}

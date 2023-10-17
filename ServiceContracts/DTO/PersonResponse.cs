using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
  public class PersonResponse
  {
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public double? Age { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? CountryName { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    public override bool Equals(object? obj)
    {
      if (obj == null) return false;
      if (obj.GetType() != typeof(PersonResponse)) return false;
      PersonResponse personResponse = (PersonResponse)obj;

      return personResponse.PersonID == PersonID && personResponse.PersonName == PersonName && personResponse.Email == Email && personResponse.DateOfBirth == DateOfBirth && personResponse.Gender == Gender && personResponse.CountryID == CountryID && personResponse.Address == Address && personResponse.ReceiveNewsLetters == ReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return $"Person ID:{PersonID}\n" +
        $"Person Name:{PersonName}";
    }

    public PersonUpdateRequest ToPersonUpdateRequest()
    {
      return new PersonUpdateRequest()
      {
        PersonID = PersonID,
        PersonName = PersonName,
        Email = Email,
        DateOfBirth = DateOfBirth,
        Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
        Address = Address,
        CountryID = CountryID,
        ReceiveNewsLetters = ReceiveNewsLetters
      };
    }
  }

  public static class PersonExtensions
  {
    public static PersonResponse ToPersonResponse(this Person person)
    {
      return new PersonResponse()
      {
        PersonID = person.PersonID,
        PersonName = person.PersonName,
        Email = person.Email,
        DateOfBirth = person.DateOfBirth,
        Address = person.Address,
        CountryID = person.CountryID,
        Gender = person.Gender,
        ReceiveNewsLetters = person.ReceiveNewsLetters,
        Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - (DateTime)person.DateOfBirth).TotalDays / 365.25) : null,
        CountryName = person.Country?.CountryName
      };
    }
  }
}

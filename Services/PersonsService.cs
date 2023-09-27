using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
  public class PersonsService : IPersonsService
  {
    private readonly PersonsDbContext _db;
    private readonly ICountriesService _countriesService;


    public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
    {
      _db = personsDbContext;
      _countriesService = countriesService;
    }

    // Util so private class
    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
      PersonResponse personResponse = person.ToPersonResponse();
      personResponse.CountryName = _countriesService.GetCountryByID(person.CountryID)?.CountryName;
      return personResponse;
    }

    public PersonResponse AddPerson(PersonAddRequest? request)
    {
      if (request == null) throw new ArgumentNullException(nameof(request));

      ValidationHelper.ModelValidation(request);
      Person person = request.ToPerson();
      person.PersonID = Guid. NewGuid();
      _db.Persons.Add(person);
      return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
      return _persons.Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
    }

    public PersonResponse? GetPersonByID(Guid? id)
    {
      if (id == null) return null;
      Person? person = _persons.FirstOrDefault(person => person.PersonID == id);
      if(person== null) return null;
      return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
      List<PersonResponse> allPersons = GetAllPersons();
      List<PersonResponse> filteredPersons = allPersons;
      if (string.IsNullOrEmpty(searchString)) return filteredPersons;
      switch (searchBy)
      {
        case nameof(PersonResponse.PersonName): filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.PersonName) ? person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(PersonResponse.Email):
          filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.Email) ? person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(PersonResponse.DateOfBirth):
          filteredPersons = allPersons.Where(person => person.DateOfBirth != null ? person.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(PersonResponse.Gender): filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.Gender) ? person.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(PersonResponse.CountryName): filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.CountryName) ? person.CountryName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(PersonResponse.Address): filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.Address) ? person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        default: break;
      }
      return filteredPersons;

    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder)
    {
      if (sortBy == null)
      {
        return persons;
      }

      List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
      {
        (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => persons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Email), SortOrderOptions.ASC) => persons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Email), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => persons.OrderBy(person => person.DateOfBirth).ToList(),

        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.DateOfBirth).ToList(),

        (nameof(PersonResponse.Age), SortOrderOptions.ASC) => persons.OrderBy(person => person.Age).ToList(),

        (nameof(PersonResponse.Age), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.Age).ToList(),

        (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => persons.OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.Gender).ToList(),

        (nameof(PersonResponse.CountryName), SortOrderOptions.ASC) => persons.OrderBy(person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.CountryName), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Address), SortOrderOptions.ASC) => persons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Address), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => persons.OrderBy(person => person.ReceiveNewsLetters).ToList(),

        (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),

        _ => persons
      };

      return sortedPersons;
    }

    public PersonResponse UpdatePerson(PersonUpdateRequest? request)
    {
      if (request == null) throw new ArgumentNullException(nameof(request));

      ValidationHelper.ModelValidation(request);

      Person? matchingPerson = _persons.FirstOrDefault(person => person.PersonID == request.PersonID);

      if (matchingPerson == null)
      {
        throw new ArgumentException("PersonID does not exist");
      }

      matchingPerson.PersonName = request.PersonName;

      matchingPerson.Email = request.Email;

      matchingPerson.DateOfBirth = request.DateOfBirth;

      matchingPerson.Gender = request.Gender.ToString();

      matchingPerson.CountryID = request.CountryID;

      matchingPerson.Address = request.Address;

      matchingPerson.ReceiveNewsLetters = request.ReceiveNewsLetters;

      return ConvertPersonToPersonResponse(matchingPerson);
    }

    public bool DeletePerson(Guid? PersonID)
    {
      if (PersonID == null)
      {
        throw new ArgumentNullException(nameof(PersonID));
      }

      Person? foundPerson = _persons.FirstOrDefault(person => person.PersonID == PersonID);

      if (foundPerson == null) return false;

      return _persons.RemoveAll(person => person.PersonID == PersonID) == 1;

    }
  }
}

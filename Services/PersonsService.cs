using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
  public class PersonsService : IPersonsService
  {
    private readonly List<Person> _persons;
    private readonly ICountriesService _countriesService;


    public PersonsService(bool initialize = true)
    {
      _persons = new List<Person>();
      _countriesService = new CountriesService();
      if (initialize)
      {
        _persons.Add(new Person() { PersonID = Guid.Parse("8082ED0C-396D-4162-AD1D-29A13F929824"), PersonName = "Aguste", Email = "aleddy0@booking.com", DateOfBirth = DateTime.Parse("1993-01-02"), Gender = "Male", Address = "0858 Novick Terrace", ReceiveNewsLetters = false, CountryID = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B") });

        _persons.Add(new Person() { PersonID = Guid.Parse("06D15BAD-52F4-498E-B478-ACAD847ABFAA"), PersonName = "Jasmina", Email = "jsyddie1@miibeian.gov.cn", DateOfBirth = DateTime.Parse("1991-06-24"), Gender = "Female", Address = "0742 Fieldstone Lane", ReceiveNewsLetters = true, CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F") });

        _persons.Add(new Person() { PersonID = Guid.Parse("D3EA677A-0F5B-41EA-8FEF-EA2FC41900FD"), PersonName = "Kendall", Email = "khaquard2@arstechnica.com", DateOfBirth = DateTime.Parse("1993-08-13"), Gender = "Male", Address = "7050 Pawling Alley", ReceiveNewsLetters = false, CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F") });

        _persons.Add(new Person() { PersonID = Guid.Parse("89452EDB-BF8C-4283-9BA4-8259FD4A7A76"), PersonName = "Kilian", Email = "kaizikowitz3@joomla.org", DateOfBirth = DateTime.Parse("1991-06-17"), Gender = "Male", Address = "233 Buhler Junction", ReceiveNewsLetters = true, CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E") });

        _persons.Add(new Person() { PersonID = Guid.Parse("F5BD5979-1DC1-432C-B1F1-DB5BCCB0E56D"), PersonName = "Dulcinea", Email = "dbus4@pbs.org", DateOfBirth = DateTime.Parse("1996-09-02"), Gender = "Female", Address = "56 Sundown Point", ReceiveNewsLetters = false, CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E") });

        _persons.Add(new Person() { PersonID = Guid.Parse("A795E22D-FAED-42F0-B134-F3B89B8683E5"), PersonName = "Corabelle", Email = "cadams5@t-online.de", DateOfBirth = DateTime.Parse("1993-10-23"), Gender = "Female", Address = "4489 Hazelcrest Place", ReceiveNewsLetters = false, CountryID = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D") });

        _persons.Add(new Person() { PersonID = Guid.Parse("3C12D8E8-3C1C-4F57-B6A4-C8CAAC893D7A"), PersonName = "Faydra", Email = "fbischof6@boston.com", DateOfBirth = DateTime.Parse("1996-02-14"), Gender = "Female", Address = "2010 Farragut Pass", ReceiveNewsLetters = true, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

        _persons.Add(new Person() { PersonID = Guid.Parse("7B75097B-BFF2-459F-8EA8-63742BBD7AFB"), PersonName = "Oby", Email = "oclutheram7@foxnews.com", DateOfBirth = DateTime.Parse("1992-05-31"), Gender = "Male", Address = "2 Fallview Plaza", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

        _persons.Add(new Person() { PersonID = Guid.Parse("6717C42D-16EC-4F15-80D8-4C7413E250CB"), PersonName = "Seumas", Email = "ssimonitto8@biglobe.ne.jp", DateOfBirth = DateTime.Parse("1999-02-02"), Gender = "Male", Address = "76779 Norway Maple Crossing", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

        _persons.Add(new Person() { PersonID = Guid.Parse("6E789C86-C8A6-4F18-821C-2ABDB2E95982"), PersonName = "Freemon", Email = "faugustin9@vimeo.com", DateOfBirth = DateTime.Parse("1996-04-27"), Gender = "Male", Address = "8754 Becker Street", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

      }
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
      person.PersonID = Guid.NewGuid();
      _persons.Add(person);
      return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
      return _persons.Select(temp => temp.ToPersonResponse()).ToList();
    }

    public PersonResponse? GetPersonByID(Guid? id)
    {
      if (id == null) return null;
      Person? person = _persons.FirstOrDefault(person => person.PersonID == id);

      return person?.ToPersonResponse();

    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
      List<PersonResponse> allPersons = GetAllPersons();
      List<PersonResponse> filteredPersons = allPersons;
      if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) return filteredPersons;
      switch (searchBy)
      {
        case nameof(Person.PersonName): filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.PersonName) ? person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(Person.Email):
          filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.Email) ? person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(Person.DateOfBirth):
          filteredPersons = allPersons.Where(person => person.DateOfBirth != null ? person.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(Person.Gender): filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.Gender) ? person.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(Person.CountryID): filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.CountryName) ? person.CountryName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

        case nameof(Person.Address): filteredPersons = allPersons.Where(person => !string.IsNullOrEmpty(person.Address) ? person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList(); break;

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

      return matchingPerson.ToPersonResponse();
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

using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
  public interface IPersonsService
  {
    PersonResponse AddPerson(PersonAddRequest? request);
    List<PersonResponse> GetAllPersons();
    PersonResponse? GetPersonByID(Guid? id);

    List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

    List<PersonResponse> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder);

     PersonResponse UpdatePerson(PersonUpdateRequest? request);

    bool DeletePerson(Guid? PersonID);



  }
}
 
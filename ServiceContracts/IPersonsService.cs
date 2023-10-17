using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
  public interface IPersonsService
  {
    Task<PersonResponse> AddPerson(PersonAddRequest? request);

    Task<List<PersonResponse>> GetAllPersons();

    Task<PersonResponse?> GetPersonByID(Guid? id);

    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

    Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder);

    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request);

    Task<bool> DeletePerson(Guid? PersonID);

    Task<MemoryStream> GetPersonsCSV();

    Task<MemoryStream> GetPersonsExcel();

  }
}

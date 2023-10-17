using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
  public interface IPersonsRepository
  {
    Task<Person> AddPerson(Person person);

    Task<List<Person>> GetAllPersons();

    Task<Person?> GetPersonByID(Guid id);

    Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

    Task<Person> UpdatePerson(Person person);

    Task<bool> DeletePerson(Guid PersonID);
  }
}

using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
  public class PersonsRepository : IPersonsRepository
  {
    private readonly ApplicationDbContext _db;

    public PersonsRepository(ApplicationDbContext db)
    {
      _db = db;
    }

    public async Task<Person> AddPerson(Person person)
    {
      _db.Persons.Add(person);
      await _db.SaveChangesAsync();
      return person;
    }

    public async Task<bool> DeletePerson(Guid PersonID)
    {
      _db.Persons.RemoveRange(_db.Persons.Where(person => person.PersonID == PersonID));
      return await _db.SaveChangesAsync() > 0;
    }

    public async Task<List<Person>> GetAllPersons()
    {
      return await _db.Persons.Include("Country").ToListAsync();
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
      return await _db.Persons.Include("Country")
        .Where(predicate).ToListAsync();
    }

    public async Task<Person?> GetPersonByID(Guid id)
    {
      return await _db.Persons.Include("Country")
        .FirstOrDefaultAsync(person => person.PersonID == id);
    }

    public async Task<Person> UpdatePerson(Person person)
    {
      Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);

      if (matchingPerson == null)
      {
        return person;
      }

      matchingPerson.PersonName = person.PersonName;

      matchingPerson.Email = person.Email;

      matchingPerson.Gender = person.Gender;

      matchingPerson.DateOfBirth = person.DateOfBirth;

      matchingPerson.CountryID = person.CountryID;

      matchingPerson.Address = person.Address;

      matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

      await _db.SaveChangesAsync();

      return matchingPerson;

    }
  }
}

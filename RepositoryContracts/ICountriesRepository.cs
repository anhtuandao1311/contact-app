using Entities;

namespace RepositoryContracts
{
  public interface ICountriesRepository
  {
    Task<Country> AddCountry(Country country);

    Task<List<Country>> GetAllCountries();

    Task<Country?> GetCountryByID(Guid id);

    Task<Country?> GetCountryByCountryName(string countryName);

  }
}
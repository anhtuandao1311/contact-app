using ServiceContracts.DTO;

namespace ServiceContracts
{
  public interface ICountriesService
  {
    CountryResponse AddCountry(CountryAddRequest? request);
    List<CountryResponse> GetAllCountries();

    CountryResponse? GetCountryByID(Guid? id);
  }
}
using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
  public interface ICountriesService
  {
    Task<CountryResponse> AddCountry(CountryAddRequest? request);

    Task<List<CountryResponse>> GetAllCountries();

    Task<CountryResponse?> GetCountryByID(Guid? id);

    Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
  }
}
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
  public class CountriesService : ICountriesService
  {

    private readonly List<Country> _countries;
    public CountriesService()
    {
      _countries = new List<Country>();
    }
    public CountryResponse AddCountry(CountryAddRequest? request)
    {
      if (request == null)
      {
        throw new ArgumentNullException(nameof(request));
      }
      if (request.CountryName == null)
      {
        throw new ArgumentException(nameof(request.CountryName));
      }
      if (_countries.Where(country => country.CountryName == request.CountryName).Any())
      {
        throw new ArgumentException("Country name already exists");
      }
      var country = request.ToCountry();
      country.CountryID = Guid.NewGuid();
      _countries.Add(country);
      return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
      return _countries.Select(country => country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByID(Guid? id)
    {
      if (id == null) return null;
      Country? foundCountry = _countries.FirstOrDefault(country => country.CountryID == id);
      return foundCountry?.ToCountryResponse();
    }
  }
}
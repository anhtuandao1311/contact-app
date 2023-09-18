using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
  public class CountriesService : ICountriesService
  {

    private readonly List<Country> _countries;
    public CountriesService(bool initialize = true)
    {
      _countries = new List<Country>();
      if (initialize)
      {
        _countries.AddRange(new List<Country>()
        {
        new Country() { CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F"),CountryName = "USA" },

        new Country() { CountryID = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B"), CountryName = "Canada" },

        new Country() { CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E"), CountryName = "UK" },

        new Country() { CountryID = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D"), CountryName = "Australia" },

        new Country() { CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB"), CountryName = "Viet Nam" }
        });
      }
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
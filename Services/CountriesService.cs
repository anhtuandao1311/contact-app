using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
  public class CountriesService : ICountriesService
  {

    private readonly PersonsDbContext _db;
    public CountriesService(PersonsDbContext personsDbContext)
    {
      _db = personsDbContext;
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
      if (_db.Countries.Where(country => country.CountryName == request.CountryName).Any())
      {
        throw new ArgumentException("Country name already exists");
      }
      var country = request.ToCountry();
      country.CountryID = Guid.NewGuid();

      _db.Countries.Add(country);
      _db.SaveChanges();

      return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
      return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByID(Guid? id)
    {
      if (id == null) return null;
      Country? foundCountry = _db.Countries.FirstOrDefault(country => country.CountryID == id);
      return foundCountry?.ToCountryResponse();
    }
  }
}
namespace ContactApp.Tests
{
  public class CountriesServiceTest
  {
    private readonly ICountriesService _countriesService;
    public CountriesServiceTest()
    {
      _countriesService = new CountriesService(false);
    }

    #region AddCountry
    [Fact]
    public void AddCountry_NullCountry()
    {
      //Arrange
      CountryAddRequest? request = null;

      //Assert
      Assert.Throws<ArgumentNullException>(() =>
      //Act
      _countriesService.AddCountry(request));

    }

    [Fact]
    public void AddCountry_NullCountryName()
    {
      //Arrange
      CountryAddRequest? request = new() { CountryName = null };

      //Assert
      Assert.Throws<ArgumentException>(() =>
      //Act
      _countriesService.AddCountry(request));

    }

    [Fact]
    public void AddCountry_DuplicateCountryName()
    {
      //Arrange
      CountryAddRequest? request1 = new() { CountryName = "USA" };
      CountryAddRequest? request2 = new() { CountryName = "USA" };


      //Assert
      Assert.Throws<ArgumentException>(() =>
      //Act
      {
        _countriesService.AddCountry(request1);
        _countriesService.AddCountry(request2);
      });
    }

    [Fact]
    public void AddCountry_ProperCountryName()
    {
      //Arrange
      CountryAddRequest? request = new() { CountryName = "USA" };
      CountryResponse? response = _countriesService.AddCountry(request);
      List<CountryResponse> allCountries = _countriesService.GetAllCountries();

      Assert.True(response != null);
      Assert.Contains(response, allCountries);
    }
    #endregion
    #region GetAllCountries
    [Fact]
    public void GetAllCountries_EmptyList()
    {
      List<CountryResponse> responseList = _countriesService.GetAllCountries();

      Assert.Empty(responseList);
    }

    [Fact]
    public void GetAllCountries_AddFewCountries()
    {
      List<CountryAddRequest?> requestList = new()
      {
        new CountryAddRequest {CountryName = "USA"},
        new CountryAddRequest { CountryName = "UK"}
      };

      List<CountryResponse> countriesFromRequestList = new();
      foreach (var request in requestList)
      {
        countriesFromRequestList.Add(_countriesService.AddCountry(request));
      }
      List<CountryResponse> actualResponse = _countriesService.GetAllCountries();

      foreach (var countries in countriesFromRequestList)
      {
        Assert.Contains(countries, actualResponse);
      }
    }
    #endregion
    #region GetCountryByID
    [Fact]
    public void GetCountryByID_NullID()
    {
      Guid? id = null;
      CountryResponse? response = _countriesService.GetCountryByID(id);

      Assert.Null(response);
    }

    [Fact]
    public void GetCountryByID_ProperID()
    {
      CountryAddRequest addRequest = new()
      {
        CountryName = "China"
      };
      CountryResponse responseFromAdd = _countriesService.AddCountry(addRequest);
      CountryResponse? responseFromGet = _countriesService.GetCountryByID(responseFromAdd.CountryID);
      Assert.Equal(responseFromAdd,responseFromGet);
    }
    #endregion
  }
}
namespace ContactApp.Tests
{
  public class CountriesServiceTest
  {
    private readonly ICountriesService _countriesService;
    public CountriesServiceTest()
    {
      var countriesInitialData = new List<Country>() { };

      // MOCKING
      DbContextMock<ApplicationDbContext> dbContextMock = new(
        new DbContextOptionsBuilder<ApplicationDbContext>().Options
        );
      ApplicationDbContext dbContext = dbContextMock.Object;
      dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

      _countriesService = new CountriesService(null);
    }

    #region AddCountry
    [Fact]
    public async Task AddCountry_NullCountry() // return task = return async Task
    {
      //Arrange
      CountryAddRequest? request = null;

      //Assert
      await Assert.ThrowsAsync<ArgumentNullException>(async () =>
      //Act
      await _countriesService.AddCountry(request));

    }

    [Fact]
    public async Task AddCountry_NullCountryName()
    {
      //Arrange
      CountryAddRequest? request = new() { CountryName = null };

      //Assert
      await Assert.ThrowsAsync<ArgumentException>(async () =>
      //Act
      await _countriesService.AddCountry(request));

    }

    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
      //Arrange
      CountryAddRequest? request1 = new() { CountryName = "USA" };
      CountryAddRequest? request2 = new() { CountryName = "USA" };


      //Assert
      await Assert.ThrowsAsync<ArgumentException>(async () =>
      //Act
     {
       await _countriesService.AddCountry(request1);
       await _countriesService.AddCountry(request2);
     });
    }

    [Fact]
    public async Task AddCountry_ProperCountryName()
    {
      //Arrange
      CountryAddRequest? request = new() { CountryName = "USA" };
      CountryResponse? response = await _countriesService.AddCountry(request);
      List<CountryResponse> allCountries = await _countriesService.GetAllCountries();

      Assert.True(response != null);
      Assert.Contains(response, allCountries);
    }
    #endregion
    #region GetAllCountries
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
      List<CountryResponse> responseList = await _countriesService.GetAllCountries();

      Assert.Empty(responseList);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
      List<CountryAddRequest?> requestList = new()
      {
        new CountryAddRequest {CountryName = "USA"},
        new CountryAddRequest { CountryName = "UK"}
      };

      List<CountryResponse> countriesFromRequestList = new();
      foreach (var request in requestList)
      {
        countriesFromRequestList.Add(await _countriesService.AddCountry(request));
      }
      List<CountryResponse> actualResponse = await _countriesService.GetAllCountries();

      foreach (var countries in countriesFromRequestList)
      {
        Assert.Contains(countries, actualResponse);
      }
    }
    #endregion
    #region GetCountryByID
    [Fact]
    public async Task GetCountryByID_NullID()
    {
      Guid? id = null;
      CountryResponse? response = await _countriesService.GetCountryByID(id);

      Assert.Null(response);
    }

    [Fact]
    public async Task GetCountryByID_ProperID()
    {
      CountryAddRequest addRequest = new()
      {
        CountryName = "China"
      };
      CountryResponse responseFromAdd = await _countriesService.AddCountry(addRequest);
      CountryResponse? responseFromGet = await _countriesService.GetCountryByID(responseFromAdd.CountryID);
      Assert.Equal(responseFromAdd, responseFromGet);
    }
    #endregion
  }
}
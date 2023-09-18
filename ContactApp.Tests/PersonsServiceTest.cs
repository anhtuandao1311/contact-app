using ServiceContracts.Enums;

namespace ContactApp.Tests
{
  public class PersonsServiceTest
  {
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
      _personsService = new PersonsService();
      _countriesService = new CountriesService(false);
      _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    [Fact]
    public void AddPerson_NullPerson()
    {
      PersonAddRequest? request = null;
      Assert.Throws<ArgumentNullException>(() =>
      _personsService.AddPerson(request));
    }

    [Fact]
    public void AddPerson_NullPersonName()
    {
      PersonAddRequest? request = new()
      {
        PersonName = "Abc"
      };
      Assert.Throws<ArgumentException>(() =>
      _personsService.AddPerson(request));
    }

    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
      PersonAddRequest? request = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = Guid.NewGuid(),
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };
      PersonResponse response = _personsService.AddPerson(request);
      Assert.True(response != null);
      List<PersonResponse> personList = _personsService.GetAllPersons();
      Assert.Contains(response, personList);
    }
    #endregion
    #region GetAllPersons
    [Fact]
    public void GetAllPersons_EmptyList()
    {
      List<PersonResponse> personList = _personsService.GetAllPersons();
      Assert.Empty(personList);
    }

    [Fact]
    public void GetAllPersons_AddSomePersons()
    {
      CountryAddRequest countryAddRequest1 = new()
      {
        CountryName = "USA"
      };
      CountryAddRequest countryAddRequest2 = new()
      {
        CountryName = "UK"
      };
      CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
      CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

      PersonAddRequest? personAddRequest1 = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = countryResponse1.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };

      PersonAddRequest? personAddRequest2 = new()
      {
        PersonName = "Nick",
        Email = "Nick@gmail.com",
        Address = "sample",
        CountryID = countryResponse2.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };
      List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2 };

      List<PersonResponse> personResponses = new List<PersonResponse>();

      foreach (var personAddRequest in personAddRequests)
      {
        personResponses.Add(_personsService.AddPerson(personAddRequest));
      }

      _testOutputHelper.WriteLine("Expected:");
      foreach (var personResponse in personResponses)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      List<PersonResponse> personResponsesFromGet = _personsService.GetAllPersons();

      foreach (var personResponse in personResponses)
      {
        Assert.Contains(personResponse, personResponsesFromGet);
      }
      _testOutputHelper.WriteLine("Actual:");
      foreach (var personResponse in personResponsesFromGet)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }
    }
    #endregion
    #region GetPersonByPersonID
    [Fact]
    public void GetPersonByID_NullPersonID()
    {
      PersonResponse? response = _personsService.GetPersonByID(null);

      Assert.Null(response);
    }

    [Fact]
    public void GetPersonByID_ProperPersonID()
    {
      CountryAddRequest countryAddRequest = new()
      {
        CountryName = "USA"
      };
      CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

      PersonAddRequest? personAddRequest = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = countryResponse.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };
      PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

      PersonResponse? personResponseFromGet = _personsService.GetPersonByID(personResponse.PersonID);

      Assert.Equal(personResponse, personResponseFromGet);


    }
    #endregion
    #region GetFilteredPersons
    [Fact]
    public void GetFilteredPersons_EmptySearchString()
    {
      CountryAddRequest countryAddRequest1 = new()
      {
        CountryName = "USA"
      };
      CountryAddRequest countryAddRequest2 = new()
      {
        CountryName = "UK"
      };
      CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
      CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

      PersonAddRequest? personAddRequest1 = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = countryResponse1.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };

      PersonAddRequest? personAddRequest2 = new()
      {
        PersonName = "Nick",
        Email = "Nick@gmail.com",
        Address = "sample",
        CountryID = countryResponse2.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };
      List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2 };

      List<PersonResponse> personResponses = new List<PersonResponse>();

      foreach (var personAddRequest in personAddRequests)
      {
        personResponses.Add(_personsService.AddPerson(personAddRequest));
      }

      _testOutputHelper.WriteLine("Expected:");
      foreach (var personResponse in personResponses)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      List<PersonResponse> personResponsesFromSearch = _personsService.GetFilteredPersons(nameof(Person.PersonName), null);

      foreach (var personResponse in personResponses)
      {
        Assert.Contains(personResponse, personResponsesFromSearch);
      }
      _testOutputHelper.WriteLine("Actual:");
      foreach (var personResponse in personResponsesFromSearch)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }
    }

    [Fact]
    public void GetFilteredPersons_SearchByPersonName()
    {
      CountryAddRequest countryAddRequest1 = new()
      {
        CountryName = "USA"
      };
      CountryAddRequest countryAddRequest2 = new()
      {
        CountryName = "UK"
      };
      CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
      CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

      PersonAddRequest? personAddRequest1 = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = countryResponse1.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };

      PersonAddRequest? personAddRequest2 = new()
      {
        PersonName = "Nick",
        Email = "Nick@gmail.com",
        Address = "sample",
        CountryID = countryResponse2.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };
      List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2 };

      List<PersonResponse> personResponses = new List<PersonResponse>();

      foreach (var personAddRequest in personAddRequests)
      {
        personResponses.Add(_personsService.AddPerson(personAddRequest));
      }

      _testOutputHelper.WriteLine("Expected:");
      foreach (var personResponse in personResponses)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      List<PersonResponse> personResponsesFromSearch = _personsService.GetFilteredPersons(nameof(Person.PersonName), "ni");

      foreach (var personResponse in personResponses)
      {
        if (personResponse.PersonName != null)
        {
          if (personResponse.PersonName.Contains("ni", StringComparison.OrdinalIgnoreCase))
          {
            Assert.Contains(personResponse, personResponsesFromSearch);
          }
        }

      }
      _testOutputHelper.WriteLine("Actual:");
      foreach (var personResponse in personResponsesFromSearch)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }
    }
    #endregion
    #region GetSortedPersons
    [Fact]
    public void GetSortedPersons_DescendingPersonName()
    {
      CountryAddRequest countryAddRequest1 = new()
      {
        CountryName = "USA"
      };
      CountryAddRequest countryAddRequest2 = new()
      {
        CountryName = "UK"
      };
      CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
      CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

      PersonAddRequest? personAddRequest1 = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = countryResponse1.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };

      PersonAddRequest? personAddRequest2 = new()
      {
        PersonName = "Nick",
        Email = "Nick@gmail.com",
        Address = "sample",
        CountryID = countryResponse2.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };

      PersonAddRequest? personAddRequest3 = new()
      {
        PersonName = "Vicky",
        Email = "Vicky@gmail.com",
        Address = "sample",
        CountryID = countryResponse2.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };
      List<PersonAddRequest> personAddRequests = new() { personAddRequest1, personAddRequest2, personAddRequest3 };

      List<PersonResponse> personResponses = new();

      foreach (var personAddRequest in personAddRequests)
      {
        personResponses.Add(_personsService.AddPerson(personAddRequest));
      }

      personResponses = personResponses.OrderByDescending(person => person.PersonName).ToList();

      _testOutputHelper.WriteLine("Expected:");
      foreach (var personResponse in personResponses)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      List<PersonResponse> allPersons = _personsService.GetAllPersons();

      List<PersonResponse> personResponsesFromSort = _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

      _testOutputHelper.WriteLine("Actual:");
      foreach (var personResponse in personResponsesFromSort)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      for (int i = 0; i < personResponses.Count; i++)
      {
        Assert.Equal(personResponses[i], personResponsesFromSort[i]);
      }


    }
    #endregion
    #region UpdatePerson
    [Fact]
    public void UpdatePerson_NullPerson()
    {
      PersonUpdateRequest? personUpdateRequest = null;

      Assert.Throws<ArgumentNullException>(() =>
      {
        _personsService.UpdatePerson(personUpdateRequest);
      });
    }

    [Fact]
    public void UpdatePerson_InvalidPersonID()
    {
      PersonUpdateRequest? personUpdateRequest = new()
      {
        PersonID = Guid.NewGuid()
      };

      Assert.Throws<ArgumentException>(() =>
      {
        _personsService.UpdatePerson(personUpdateRequest);
      });
    }

    [Fact]
    public void UpdatePerson_NullPersonName()
    {
      PersonUpdateRequest? personUpdateRequest = new()
      {
        PersonName = null
      };

      Assert.Throws<ArgumentException>(() =>
      {
        _personsService.UpdatePerson(personUpdateRequest);
      });
    }

    [Fact]
    public void UpdatePerson_ProperUpdate()
    {
      CountryAddRequest countryAddRequest = new()
      {
        CountryName = "USA"
      };
      CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
      PersonAddRequest? personAddRequest = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = countryResponse.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };

      PersonResponse responseFromAdd = _personsService.AddPerson(personAddRequest);

      PersonUpdateRequest updateRequest = responseFromAdd.ToPersonUpdateRequest();

      updateRequest.PersonName = "William";
      updateRequest.Email = "William@gmail.com";

      PersonResponse responseFromUpdate = _personsService.UpdatePerson(updateRequest);

      PersonResponse? responseFromGet = _personsService.GetPersonByID(responseFromUpdate.PersonID);

      Assert.Equal(responseFromGet, responseFromUpdate);
    }
    #endregion
    #region DeletePerson
    [Fact]
    public void DeletePerson_InvalidPersonID()
    {
      CountryAddRequest countryAddRequest = new()
      {
        CountryName = "USA"
      };
      CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
      PersonAddRequest? personAddRequest = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = countryResponse.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };

      PersonResponse responseFromAdd = _personsService.AddPerson(personAddRequest);

      bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());

      Assert.False(isDeleted);
    }

    [Fact]
    public void DeletePerson_ValidPersonID()
    {
      CountryAddRequest countryAddRequest = new()
      {
        CountryName = "USA"
      };
      CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
      PersonAddRequest? personAddRequest = new()
      {
        PersonName = "John",
        Email = "john@gmail.com",
        Address = "sample",
        CountryID = countryResponse.CountryID,
        Gender = GenderOptions.Male,
        DateOfBirth = DateTime.Parse("2000-01-01"),
        ReceiveNewsLetters = true
      };

      PersonResponse responseFromAdd = _personsService.AddPerson(personAddRequest);

      bool isDeleted = _personsService.DeletePerson(responseFromAdd.PersonID);

      Assert.True(isDeleted);
    }
    #endregion
  }
}

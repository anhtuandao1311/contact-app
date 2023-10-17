
using Entities;
using ServiceContracts.Enums;
using System.Linq.Expressions;

namespace ContactApp.Tests
{
  public class PersonsServiceTest
  {
    private readonly IPersonsService _personsService;
    private readonly IPersonsRepository _personsRepository;
    private readonly Mock<IPersonsRepository> _personsRepositoryMock;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
      // AUTOFIXTURE
      _fixture = new Fixture();
      // MOCKING
      _personsRepositoryMock = new Mock<IPersonsRepository>();
      _personsRepository = _personsRepositoryMock.Object;

      _personsService = new PersonsService(_personsRepository);
      _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
      PersonAddRequest? request = null;
      Func<Task> action = async () =>
       await _personsService.AddPerson(request);
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddPerson_NullPersonName_ToBeArgumentException()
    {
      PersonAddRequest? request = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

      // return the same "person" object when repo.AddPerson is called
      Person person = request.ToPerson();
      _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

      Func<Task> action = async () =>
      await _personsService.AddPerson(request);
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
      // AUTOFIXTURE
      PersonAddRequest? request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "ex@gmail.com").Create();

      Person person = request.ToPerson();
      PersonResponse response_expected = person.ToPersonResponse();

      // mock repo
      _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

      PersonResponse response = await _personsService.AddPerson(request);
      response_expected.PersonID = response.PersonID;
      //Assert.True(response != null);
      // == : Be, != NotBe
      response.Should().NotBeNull();
      response.Should().Be(response_expected);
    }
    #endregion
    #region GetAllPersons
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
      List<Person> personList = new List<Person>();
      _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(personList);

      List<PersonResponse> responseList = await _personsService.GetAllPersons();

      //Assert.Empty(personList);
      responseList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_WithSomePersons_ToBeSuccessful()
    {
      List<Person> persons = new List<Person>() {
        _fixture.Build<Person>().With(temp => temp.Email, "ex1@gmail.com").Create(),
        _fixture.Build<Person>().With(temp => temp.Email, "ex2@gmail.com").Create(),
        _fixture.Build<Person>().With(temp => temp.Email, "ex3@gmail.com").Create()
       };

      List<PersonResponse> personResponsesExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

      _testOutputHelper.WriteLine("Expected:");
      foreach (var personResponse in personResponsesExpected)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

      List<PersonResponse> personResponsesFromGet = await _personsService.GetAllPersons();

      //foreach (var personResponse in personResponses)
      //{
      //  Assert.Contains(personResponse, personResponsesFromGet);
      //}

      _testOutputHelper.WriteLine("Actual:");
      foreach (var personResponse in personResponsesFromGet)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      personResponsesFromGet.Should().BeEquivalentTo(personResponsesExpected);
    }
    #endregion
    #region GetPersonByPersonID
    [Fact]
    public async Task GetPersonByID_NullPersonID_ToBeNull()
    {
      PersonResponse? response = await _personsService.GetPersonByID(null);

      //Assert.Null(response);
      response.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonByID_ProperPersonID_ToBeSuccessful()
    {

      Person? person = _fixture.Build<Person>().With(temp => temp.Email, "ex@gmail.com").Create();
      PersonResponse personResponse = person.ToPersonResponse();

      _personsRepositoryMock.Setup(temp => temp.GetPersonByID(It.IsAny<Guid>())).ReturnsAsync(person);

      PersonResponse? personResponseFromGet = await _personsService.GetPersonByID(personResponse.PersonID);

      //Assert.Equal(personResponse, personResponseFromGet);
      personResponseFromGet.Should().Be(personResponse);


    }
    #endregion
    #region GetFilteredPersons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchString_ToBeSuccessful()
    {
      List<Person> persons = new List<Person>() {
        _fixture.Build<Person>().With(temp => temp.Email, "ex1@gmail.com").Create(),
        _fixture.Build<Person>().With(temp => temp.Email, "ex2@gmail.com").Create(),
        _fixture.Build<Person>().With(temp => temp.Email, "ex3@gmail.com").Create()
       };

      List<PersonResponse> personResponsesExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

      _testOutputHelper.WriteLine("Expected:");
      foreach (var personResponse in personResponsesExpected)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

      List<PersonResponse> personResponsesFromSearch = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "Ni");

      //foreach (var personResponse in personResponses)
      //{
      //  Assert.Contains(personResponse, personResponsesFromSearch);
      //}
      _testOutputHelper.WriteLine("Actual:");
      foreach (var personResponse in personResponsesFromSearch)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      personResponsesFromSearch.Should().BeEquivalentTo(personResponsesExpected);
    }

    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
    {
      List<Person> persons = new List<Person>() {
        _fixture.Build<Person>().With(temp => temp.Email, "ex1@gmail.com").Create(),
        _fixture.Build<Person>().With(temp => temp.Email, "ex2@gmail.com").Create(),
        _fixture.Build<Person>().With(temp => temp.Email, "ex3@gmail.com").Create()
       };

      List<PersonResponse> personResponsesExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

      _testOutputHelper.WriteLine("Expected:");
      foreach (var personResponse in personResponsesExpected)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

      List<PersonResponse> personResponsesFromSearch = await _personsService.GetFilteredPersons(nameof(Person.PersonName), null);

      //foreach (var personResponse in personResponses)
      //{
      //  Assert.Contains(personResponse, personResponsesFromSearch);
      //}
      _testOutputHelper.WriteLine("Actual:");
      foreach (var personResponse in personResponsesFromSearch)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      personResponsesFromSearch.Should().BeEquivalentTo(personResponsesExpected);
    }
    #endregion
    #region GetSortedPersons
    [Fact]
    public async Task GetSortedPersons_DescendingPersonName_ToBeSuccessful()
    {
      List<Person> persons = new List<Person>() {
        _fixture.Build<Person>().With(temp => temp.Email, "ex1@gmail.com").Create(),
        _fixture.Build<Person>().With(temp => temp.Email, "ex2@gmail.com").Create(),
        _fixture.Build<Person>().With(temp => temp.Email, "ex3@gmail.com").Create()
       };

      List<PersonResponse> personResponsesExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

      _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

      _testOutputHelper.WriteLine("Expected:");
      foreach (var personResponse in personResponsesExpected)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      List<PersonResponse> allPersons = await _personsService.GetAllPersons();

      List<PersonResponse> personResponsesFromSort = await _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

      _testOutputHelper.WriteLine("Actual:");
      foreach (var personResponse in personResponsesFromSort)
      {
        _testOutputHelper.WriteLine(personResponse.ToString());
      }

      //for (int i = 0; i < personResponses.Count; i++)
      //{
      //  Assert.Equal(personResponses[i], personResponsesFromSort[i]);
      //}

      personResponsesFromSort.Should().BeInDescendingOrder(temp => temp.PersonName);


    }
    #endregion
    #region UpdatePerson
    [Fact]
    public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
    {
      PersonUpdateRequest? personUpdateRequest = null;

      Func<Task> action = async () =>
      {
        await _personsService.UpdatePerson(personUpdateRequest);
      };

      await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
    {
      PersonUpdateRequest? personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

      Func<Task> action = async () =>
      {
        await _personsService.UpdatePerson(personUpdateRequest);
      };

      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
    {
      Person person = _fixture.Build<Person>()
        .With(temp => temp.PersonName, null as string)
        .With(temp => temp.Gender, "Male")
        .Create();

      PersonResponse personResponse = person.ToPersonResponse();

      PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

      Func<Task> action = async () =>
      {
        await _personsService.UpdatePerson(personUpdateRequest);
      };

      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdatePerson_ProperUpdate_ToBeSuccessful()
    {
      // create person to return as mock repo
      Person person = _fixture.Build<Person>()
        .With(temp => temp.Email, "ex@gmail.com")
        .With(temp => temp.Gender, "Male")
        .Create();

      PersonResponse personResponseExpected = person.ToPersonResponse();

      PersonUpdateRequest updateRequest = personResponseExpected.ToPersonUpdateRequest();

      _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);

      _personsRepositoryMock.Setup(temp => temp.GetPersonByID(It.IsAny<Guid>())).ReturnsAsync(person);

      PersonResponse responseFromUpdate = await _personsService.UpdatePerson(updateRequest);

      //Assert.Equal(responseFromGet, responseFromUpdate);
      responseFromUpdate.Should().Be(personResponseExpected);
    }
    #endregion
    #region DeletePerson
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
      _personsRepositoryMock.Setup(temp => temp.GetPersonByID(It.IsAny<Guid>())).ReturnsAsync(null as Person);

      bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

      //Assert.False(isDeleted);
      isDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
    {
      Person person = _fixture.Build<Person>()
        .With(temp => temp.Email, "ex@gmail.com")
        .With(temp => temp.Gender, "Male")
        .Create();

      _personsRepositoryMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(true);

      _personsRepositoryMock.Setup(temp => temp.GetPersonByID(It.IsAny<Guid>())).ReturnsAsync(person);

      bool isDeleted = await _personsService.DeletePerson(person.PersonID);

      //Assert.True(isDeleted);
      isDeleted.Should().BeTrue();
    }
    #endregion
  }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactApp.Controllers
{
  [Route("[controller]")]
  public class PersonsController : Controller
  {
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    public PersonsController(IPersonsService personsService, ICountriesService countriesService)
    {
      _personsService = personsService;
      _countriesService = countriesService;
    }

    [Route("[action]")]
    [Route("/")]
    public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
      ViewBag.SearchFields = new Dictionary<string, string>()
      {
        {nameof(PersonResponse.PersonName), "Person Name"},
        {nameof(PersonResponse.Email), "Email"},
        {nameof(PersonResponse.DateOfBirth), "Date of Birth"},
        {nameof(PersonResponse.Gender), "Gender"},
        {nameof(PersonResponse.CountryName), "Country"},
        {nameof(PersonResponse.Address), "Address"}
      };

      List<PersonResponse> allPersons = _personsService.GetFilteredPersons(searchBy, searchString);
      ViewBag.CurrentSearchBy = searchBy;
      ViewBag.CurrentSearchString = searchString;

      List<PersonResponse> SortedPersons = _personsService.GetSortedPersons(allPersons, sortBy, sortOrder);
      ViewBag.CurrentSortBy = sortBy;
      ViewBag.CurrentSortOrder = sortOrder.ToString();
      return View(SortedPersons);
    }

    [Route("[action]")]
    [HttpGet]
    public IActionResult Create()
    {
      List<CountryResponse> countries = _countriesService.GetAllCountries();
      ViewBag.Countries = countries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });

      return View();
    }

    [Route("[action]")]
    [HttpPost]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
      if (!ModelState.IsValid)
      {
        List<CountryResponse> countries = _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });

        ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(err => err.ErrorMessage).ToList();
        return View();
      }

      PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

      return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("[action]/{personID}")]
    public IActionResult Edit(Guid personID)
    {
      PersonResponse? personResponse = _personsService.GetPersonByID(personID);
      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

      List<CountryResponse> countries = _countriesService.GetAllCountries();

      ViewBag.Countries = countries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });

      return View(personUpdateRequest);
    }

    [HttpPost]
    [Route("[action]/{personID}")]
    public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
    {
      PersonResponse? personResponse = _personsService.GetPersonByID(personUpdateRequest.PersonID);
      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      if (ModelState.IsValid)
      {
        PersonResponse updatedPerson = _personsService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index");
      }
      else
      {
        List<CountryResponse> countries = _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });

        ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(err => err.ErrorMessage).ToList();
        return View(personResponse.ToPersonUpdateRequest());
      }
    }

    [HttpGet]
    [Route("[action]/{personID}")]
    public IActionResult Delete(Guid? personID)
    {
      PersonResponse? personResponse = _personsService.GetPersonByID(personID);
      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      return View(personResponse);
    }

    public IActionResult Delete(PersonUpdateRequest personupdateRequest)
    {
      PersonResponse? personResponse = _personsService.GetPersonByID(personupdateRequest.PersonID);
      if(personResponse == null)
      {
        return RedirectToAction("Index");
      }

      _personsService.DeletePerson(personupdateRequest.PersonID);
      return RedirectToAction("Index");
    }
  } 
}

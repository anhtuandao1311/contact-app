using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
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
    [HttpGet]
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
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

      List<PersonResponse> allPersons = await _personsService.GetFilteredPersons(searchBy, searchString);
      ViewBag.CurrentSearchBy = searchBy;
      ViewBag.CurrentSearchString = searchString;

      List<PersonResponse> SortedPersons = await _personsService.GetSortedPersons(allPersons, sortBy, sortOrder);
      ViewBag.CurrentSortBy = sortBy;
      ViewBag.CurrentSortOrder = sortOrder.ToString();
      return View(SortedPersons);
    }

    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      List<CountryResponse> countries = await _countriesService.GetAllCountries();
      ViewBag.Countries = countries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });

      return View();
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
    {
      if (!ModelState.IsValid)
      {
        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });

        ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(err => err.ErrorMessage).ToList();
        return View();
      }

      PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

      return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Edit(Guid personID)
    {
      PersonResponse? personResponse = await _personsService.GetPersonByID(personID);
      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

      List<CountryResponse> countries = await _countriesService.GetAllCountries();

      ViewBag.Countries = countries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });

      return View(personUpdateRequest);
    }

    [HttpPost]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
    {
      PersonResponse? personResponse = await _personsService.GetPersonByID(personUpdateRequest.PersonID);
      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      if (ModelState.IsValid)
      {
        PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index");
      }
      else
      {
        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });

        ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(err => err.ErrorMessage).ToList();
        return View(personResponse.ToPersonUpdateRequest());
      }
    }

    [HttpGet]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Delete(Guid? personID)
    {
      PersonResponse? personResponse = await _personsService.GetPersonByID(personID);
      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      return View(personResponse);
    }

    [HttpPost]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Delete(PersonUpdateRequest personupdateRequest)
    {
      PersonResponse? personResponse = await _personsService.GetPersonByID(personupdateRequest.PersonID);
      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      await _personsService.DeletePerson(personupdateRequest.PersonID);
      return RedirectToAction("Index");
    }

    [Route("[action]")]
    public async Task<IActionResult> PersonsPDF()
    {
      List<PersonResponse> persons = await _personsService.GetAllPersons();
      return new ViewAsPdf("PersonsPDF", persons, ViewData)
      {
        PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
      };
    }

    [Route("[action]")]
    public async Task<IActionResult> PersonsCSV()
    {
      MemoryStream memstream = await _personsService.GetPersonsCSV();
      return File(memstream, "application/octet-stream", "persons.csv");
    }

    [Route("[action]")]
    public async Task<IActionResult> PersonsExcel()
    {
      MemoryStream memstream = await _personsService.GetPersonsExcel();
      return File(memstream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
    }
  }
}

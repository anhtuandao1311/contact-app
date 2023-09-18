using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ContactApp.Controllers
{
  public class PersonsController : Controller
  {
    private readonly IPersonsService _personsService;
    public PersonsController(IPersonsService personsService)
    {
      _personsService = personsService;
    }

    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
      List<PersonResponse> allPersons = _personsService.GetAllPersons().ToList();
      return View(allPersons);
    }
  }
}

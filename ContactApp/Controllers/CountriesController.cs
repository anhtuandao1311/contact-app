using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace ContactApp.Controllers
{
  [Route("[controller]")]
  public class CountriesController : Controller
  {
    private readonly ICountriesService _countriesService;

    public CountriesController(ICountriesService countriesService)
    {
      _countriesService = countriesService;
    }

    [Route("[action]")]
    [HttpGet]
    public IActionResult UploadFromExcel()
    {
      return View();
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> UploadFromExcel(IFormFile? excelFile)
    {
      if(excelFile == null || excelFile.Length == 0)
      {
        ViewBag.ErrorMessage = "Please select an .xlsx file";
        return View();
      }

      if (Path.GetExtension(excelFile.FileName).Equals(".xlsx",StringComparison.OrdinalIgnoreCase))
      {
        ViewBag.ErrorMessage = "Unsupported file";
        return View();
      }

      int countriesInserted = await _countriesService.UploadCountriesFromExcelFile(excelFile);

      ViewBag.Message = $"{countriesInserted} countries inserted";
      return View();

    }
  }
}

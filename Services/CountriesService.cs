using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services
{
  public class CountriesService : ICountriesService
  {

    private readonly ICountriesRepository _countriesRepository;
    public CountriesService(ICountriesRepository countriesRepository)
    {
      _countriesRepository = countriesRepository;
    }
    public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
    {
      if (request == null)
      {
        throw new ArgumentNullException(nameof(request));
      }
      if (request.CountryName == null)
      {
        throw new ArgumentException(nameof(request.CountryName));
      }
      if (await _countriesRepository.GetCountryByCountryName(request.CountryName) != null)
      {
        throw new ArgumentException("Country name already exists");
      }
      var country = request.ToCountry();
      country.CountryID = Guid.NewGuid();

      await _countriesRepository.AddCountry(country);

      return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
      return (await _countriesRepository.GetAllCountries()).Select(country=>country.ToCountryResponse()).ToList();
    }

    public async Task<CountryResponse?> GetCountryByID(Guid? id)
    {
      if (id == null) return null;
      Country? foundCountry = await _countriesRepository.GetCountryByID(id.Value);
      return foundCountry?.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
      MemoryStream memStream = new();
      await formFile.CopyToAsync(memStream);
      int countriesInserted = 0;

      using (ExcelPackage excelPackage = new(memStream))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

        int rowCount = workSheet.Dimension.Rows;

        for (int row = 2; row <= rowCount; row++)
        {
          string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);
          if (cellValue != null)
          {
            if (_countriesRepository.GetCountryByCountryName(cellValue)==null)
            {
              Country country = new() { CountryName = cellValue };
              await _countriesRepository.AddCountry(country);
              countriesInserted++;
            }
          }
        }
      }
      return countriesInserted;
    }
  }
}
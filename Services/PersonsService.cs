using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;

namespace Services
{
  public class PersonsService : IPersonsService
  {
    private readonly IPersonsRepository _personsRepository;

    public PersonsService(IPersonsRepository personsRepository)
    {
      _personsRepository = personsRepository;
    }

    // Util so private class
    //private PersonResponse ConvertPersonToPersonResponse(Person person)
    //{
    //  PersonResponse personResponse = person.ToPersonResponse();
    //  personResponse.CountryName = person.Country?.CountryName;
    //  return personResponse;
    //}

    public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
    {
      if (request == null) throw new ArgumentNullException(nameof(request));

      ValidationHelper.ModelValidation(request);
      Person person = request.ToPerson();
      person.PersonID = Guid.NewGuid();
      await _personsRepository.AddPerson(person);
      return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
      //user-defined methods cant be used in LINQ to entity , must add ToList() to become List<Person> First

      List<Person> persons = await _personsRepository.GetAllPersons();

      return persons.Select(person => person.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByID(Guid? id)
    {
      if (id == null) return null;
      Person? person = await _personsRepository.GetPersonByID(id.Value);
      if (person == null) return null;
      return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
      List<Person> allPersons = searchBy switch
      {
        nameof(PersonResponse.PersonName) => await _personsRepository.GetFilteredPersons(person => person.PersonName.Contains(searchString)),

        nameof(PersonResponse.Email) => await _personsRepository.GetFilteredPersons(person => person.Email.Contains(searchString)),

        nameof(PersonResponse.DateOfBirth) => await _personsRepository.GetFilteredPersons(person => person.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

        nameof(PersonResponse.Gender) => await _personsRepository.GetFilteredPersons(person => person.Gender.Contains(searchString)),

        nameof(PersonResponse.CountryName) => await _personsRepository.GetFilteredPersons(person => person.Country.CountryName.Contains(searchString)),

        nameof(PersonResponse.Address) => await _personsRepository.GetFilteredPersons(person => person.Address.Contains(searchString)),

        _ => await _personsRepository.GetAllPersons()
      };
      return allPersons.Select(person => person.ToPersonResponse()).ToList();

    }

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder)
    {
      if (sortBy == null)
      {
        return persons;
      }

      List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
      {
        (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => persons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Email), SortOrderOptions.ASC) => persons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Email), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => persons.OrderBy(person => person.DateOfBirth).ToList(),

        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.DateOfBirth).ToList(),

        (nameof(PersonResponse.Age), SortOrderOptions.ASC) => persons.OrderBy(person => person.Age).ToList(),

        (nameof(PersonResponse.Age), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.Age).ToList(),

        (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => persons.OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.Gender).ToList(),

        (nameof(PersonResponse.CountryName), SortOrderOptions.ASC) => persons.OrderBy(person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.CountryName), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Address), SortOrderOptions.ASC) => persons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Address), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => persons.OrderBy(person => person.ReceiveNewsLetters).ToList(),

        (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => persons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),

        _ => persons
      };

      return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request)
    {
      if (request == null) throw new ArgumentNullException(nameof(request));

      ValidationHelper.ModelValidation(request);

      Person? matchingPerson = await _personsRepository.GetPersonByID(request.PersonID);

      if (matchingPerson == null)
      {
        throw new ArgumentException("PersonID does not exist");
      }

      matchingPerson.PersonName = request.PersonName;

      matchingPerson.Email = request.Email;

      matchingPerson.DateOfBirth = request.DateOfBirth;

      matchingPerson.Gender = request.Gender.ToString();

      matchingPerson.CountryID = request.CountryID;

      matchingPerson.Address = request.Address;

      matchingPerson.ReceiveNewsLetters = request.ReceiveNewsLetters;

      await _personsRepository.UpdatePerson(matchingPerson);

      return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? PersonID)
    {
      if (PersonID == null)
      {
        throw new ArgumentNullException(nameof(PersonID));
      }

      Person? foundPerson = await _personsRepository.GetPersonByID(PersonID.Value);

      if (foundPerson == null) return false;

      await _personsRepository.DeletePerson(PersonID.Value);

      return true;

    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
      MemoryStream memStream = new();
      StreamWriter streamWriter = new(memStream);
      CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
      CsvWriter csvWriter = new(streamWriter, csvConfiguration);

      csvWriter.WriteField(nameof(PersonResponse.PersonName));
      csvWriter.WriteField(nameof(PersonResponse.Email));
      csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
      csvWriter.WriteField(nameof(PersonResponse.Age));
      csvWriter.WriteField(nameof(PersonResponse.Gender));
      csvWriter.WriteField(nameof(PersonResponse.CountryName));
      csvWriter.WriteField(nameof(PersonResponse.Address));
      csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
      csvWriter.NextRecord();

      List<PersonResponse> persons = await GetAllPersons();


      foreach (PersonResponse person in persons)
      {
        csvWriter.WriteField(person.PersonName);
        csvWriter.WriteField(person.Email);
        if (person.DateOfBirth.HasValue)
        {
          csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
        }
        else
        {
          csvWriter.WriteField("");
        }
        csvWriter.WriteField(person.Age);
        csvWriter.WriteField(person.Gender);
        csvWriter.WriteField(person.CountryName);
        csvWriter.WriteField(person.Address);
        csvWriter.WriteField(person.ReceiveNewsLetters);
        csvWriter.NextRecord();
        csvWriter.Flush();
      }

      memStream.Position = 0;
      return memStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
      MemoryStream memStream = new();
      using (ExcelPackage excelPackage = new(memStream))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
        workSheet.Cells["A1"].Value = "Person Name";
        workSheet.Cells["B1"].Value = "Email";
        workSheet.Cells["C1"].Value = "Date of Birth";
        workSheet.Cells["D1"].Value = "Age";
        workSheet.Cells["E1"].Value = "Gender";
        workSheet.Cells["F1"].Value = "Country";
        workSheet.Cells["G1"].Value = "Address";
        workSheet.Cells["H1"].Value = "Receive NewsLetters";

        using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
        {
          headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
          headerCells.Style.Font.Bold = true;

        }

        int row = 2;
        List<PersonResponse> persons = await GetAllPersons();

        foreach (PersonResponse person in persons)
        {
          workSheet.Cells[row, 1].Value = person.PersonName;
          workSheet.Cells[row, 2].Value = person.Email;
          workSheet.Cells[row, 3].Value = person.DateOfBirth.HasValue ? person.DateOfBirth.Value.ToString("yyyy-MM-dd") : "";
          workSheet.Cells[row, 4].Value = person.Age;
          workSheet.Cells[row, 5].Value = person.Gender;
          workSheet.Cells[row, 6].Value = person.CountryName;
          workSheet.Cells[row, 7].Value = person.Address;
          workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

          row++;
        }

        workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

        await excelPackage.SaveAsync();
      }

      memStream.Position = 0;
      return memStream;

    }
  }
}

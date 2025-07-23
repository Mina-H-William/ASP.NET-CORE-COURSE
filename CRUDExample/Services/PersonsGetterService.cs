using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;
using System.Text;

namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;

        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagonsticContext; // For Serilog context logging

        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger,
                              IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagonsticContext = diagnosticContext;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");

            //var persons = await _db.Persons
            //    .Include(person => person.Country)// Include the Country navigation property to avoid N+1 query problem
            //    .ToListAsync(); // Fetch all persons from the database

            var persons = await _personsRepository.GetAllPersons();

            return persons.Select(person => person.ToPersonResponse()).ToList();


            // ToList() after .persons will execute the query and fetch all persons from the database
            // as we can't use my own code like (ConverPersonToPersonResponse function) in linq query on database
            //return _db.Persons.ToList().Select(person => ConvertPersonToPersonResponseWithCountry(person)).ToList();

            // // this code use stored procedure to get all persons from the database
            //return _db.sp_GetAllPersons().Select(person => ConvertPersonToPersonResponseWithCountry(person)).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID is null) return null;

            Person? person = await _personsRepository.GetPersonById(personID.Value);
            if (person is null) return null;

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");

            #region Old way of filtering with LINQ Not recommended
            //List<PersonResponse> allPersons = await GetAllPersons();
            //List<PersonResponse> matchedPersons = allPersons;

            //if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            //    return matchedPersons;

            //switch (searchBy)
            //{
            //    case nameof(PersonResponse.PersonName):
            //        matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.PersonName)) ?
            //            person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
            //        break;
            //    case nameof(PersonResponse.Email):
            //        matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Email)) ?
            //            person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
            //        break;
            //    case nameof(PersonResponse.DateOfBirth):
            //        matchedPersons = allPersons.Where(person => (person.DateOfBirth != null) ?
            //            person.DateOfBirth.Value.ToString("dd mmmm yyyy")
            //            .Contains(searchString, StringComparison.OrdinalIgnoreCase)
            //            : true)
            //            .ToList();
            //        break;
            //    case nameof(PersonResponse.Gender):
            //        matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Gender)) ?
            //            person.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
            //        break;
            //    case nameof(PersonResponse.Country):
            //        matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Country)) ?
            //            person.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
            //        break;
            //    case nameof(PersonResponse.Address):
            //        matchedPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Address)) ?
            //            person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
            //        break;
            //}
            #endregion

            #region filter with reflection
            //var property = typeof(PersonResponse).GetProperty(searchBy);

            //if (property == null)
            //    return allPersons;

            ////here i can check type of property with switch case and do search logic accordingly.
            ////.............

            //matchedPersons = allPersons.Where(person => property.GetValue(person)?.ToString()?
            //                 .Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            #endregion

            if (string.IsNullOrEmpty(searchString)) return await GetAllPersons();

            // by Default SQL when compare or use like in query it is case insensitive so no need for StringComparison
            // if property if nullable so Entity Framework will translate
            // this into SQL that handles NULLs safely — it does not throw an exception.

            List<Person> persons;

            using (Operation.Time("Time For Filtered Persons from DataBase"))
            {
                persons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                        await _personsRepository.GetFilteredPersons(person =>
                            person.PersonName!.Contains(searchString)),

                    nameof(PersonResponse.Email) =>
                        await _personsRepository.GetFilteredPersons(person =>
                            person.Email!.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                        await _personsRepository.GetFilteredPersons(person =>
                            person.DateOfBirth!.Value.ToString("dd MMMM yyyy")
                            .Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                        await _personsRepository.GetFilteredPersons(person =>
                            person.Gender!.Equals(searchString)),

                    nameof(PersonResponse.Country) =>
                        await _personsRepository.GetFilteredPersons(person =>
                            person.Country!.CountryName!.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                        await _personsRepository.GetFilteredPersons(person =>
                            person.Address!.Contains(searchString)),

                    _ => await _personsRepository.GetAllPersons() // If no valid searchBy, return all persons
                };
            }

            _diagonsticContext.Set("Persons", persons);

            return persons.Select(person => person.ToPersonResponse()).ToList();
        }

        // to write csv file all values and Headers automatically wihout any custom chnages
        public async Task<MemoryStream> GetPersonsCSVAuto()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

            csvWriter.WriteHeader<PersonResponse>(); // PersonID, PersonName, EMail, etc..
            csvWriter.NextRecord(); // Move to the next line after writing the header

            List<PersonResponse> persons = await GetAllPersons();
            await csvWriter.WriteRecordsAsync(persons); // 1,PersonName,Email, etc..

            memoryStream.Position = 0; // Reset the position of the memory stream to the beginning
            return memoryStream;
        }

        // to write csv file with custom headers and custom data format
        public async Task<MemoryStream> GetPersonsCSVCustomized()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            // writing headers one by one instead of using WriteHeader<PersonResponse>() method
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReciveNewsLetters));
            await csvWriter.NextRecordAsync();

            // writing persons data
            List<PersonResponse> persons = await GetAllPersons();
            foreach (PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                csvWriter.WriteField(person.DateOfBirth?.ToString("dd MM yyyy") ?? ""); // Format the date as needed
                csvWriter.WriteField(person.Age?.ToString() ?? "");
                csvWriter.WriteField(person.Country ?? ""); // Handle null Country
                csvWriter.WriteField(person.Address ?? ""); // Handle null Address
                csvWriter.WriteField(person.ReciveNewsLetters);

                await csvWriter.NextRecordAsync();
                csvWriter.Flush(); // Ensure the data is written to the stream (required to data be writen in file)
            }

            memoryStream.Position = 0; // Reset the position of the memory stream to the beginning
            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Receiving News Letters";

                using (ExcelRange headerCells = workSheet.Cells["A1:H1"]) // to apply styles for range of cells
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true; // Make the header bold
                }

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach (PersonResponse person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;
                    workSheet.Cells[row, 3].Value = person.DateOfBirth?.ToString("dd MM yyyy") ?? "";
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.Country;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.ReciveNewsLetters;

                    row++;
                }
                workSheet.Cells[$"A1:H{row}"].AutoFitColumns(); // Auto-fit the columns based on content

                // we can save content above in saveasasync(stream)
                //await excelPackage.SaveAsAsync(memoryStream); // Save the Excel package to the memory stream

                // or we can use Save method and when create excel package give it reference to memory stream to save it directly
                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0; // Reset the position of the memory stream to the beginning
            return memoryStream;
        }
    }
}
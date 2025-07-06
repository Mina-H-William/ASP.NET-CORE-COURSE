using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? CountryAddRequest)
        {

            if (CountryAddRequest is null)
            {
                throw new ArgumentNullException($"{nameof(CountryAddRequest)} cannot be Null");
            }

            if (CountryAddRequest.CountryName is null)
            {
                throw new ArgumentException($"{nameof(CountryAddRequest.CountryName)} cannot be null.");
            }

            if (await _countriesRepository.GetCountryByCountryName(CountryAddRequest.CountryName) != null)
                throw new ArgumentException("Country with the same name already exists.");

            Country country = CountryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();

            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID is null)
                return null;

            return (await _countriesRepository.GetCountryByCountryId(countryID.Value))?.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formfile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formfile.CopyToAsync(memoryStream);

            List<Country> countriesToAdd = new List<Country>();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? countryName = Convert.ToString(workSheet.Cells[row, 1].Value);
                    if (!string.IsNullOrEmpty(countryName))
                    {
                        if ((await _countriesRepository.GetCountryByCountryName(countryName)) == null)
                        {
                            countriesToAdd.Add(new Country()
                            {
                                CountryName = countryName
                            });
                        }
                    }
                }

                countriesToAdd.ForEach(async country => await _countriesRepository.AddCountry(country));
            }
            return countriesToAdd.Count;
        }

    }
}

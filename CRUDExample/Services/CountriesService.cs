using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        public CountriesService(bool intialize = true)
        {
            _countries = new List<Country>();
            if (intialize)
            {
                _countries.AddRange(new List<Country>()
                {
                    new Country { CountryID = Guid.Parse("EBF9A556-6EE2-4040-B400-2AF870769A12"), CountryName = "USA"},
                    new Country { CountryID = Guid.Parse("B4A76B67-1288-4388-8B0B-939A1D7051FC"), CountryName = "Egypt"},
                    new Country { CountryID = Guid.Parse("9A8FEAEF-CABE-42A4-AE8F-CA0A837CB3EC"), CountryName = "Canada"},
                    new Country { CountryID = Guid.Parse("539028FD-2120-4DFA-A287-9B6A225392AB"), CountryName = "India"},
                    new Country { CountryID = Guid.Parse("977B93EB-E134-4871-AE30-7DCE755857BB"), CountryName = "UK"},
                });
            }
        }

        public CountryResponse AddCountry(CountryAddRequest? CountryAddRequest)
        {

            if (CountryAddRequest is null)
            {
                throw new ArgumentNullException($"{nameof(CountryAddRequest)} cannot be Null");
            }

            if (CountryAddRequest.CountryName is null)
            {
                throw new ArgumentException($"{nameof(CountryAddRequest.CountryName)} cannot be null.");
            }

            Country country = CountryAddRequest.ToCountry();

            //foreach (var item in _countries)
            //{
            //    if (item.CountryName == country.CountryName)
            //        throw new ArgumentException("Country with the same name already exists.");
            //}

            if (_countries.Where(item => item.CountryName == country.CountryName).Count() > 0)
                throw new ArgumentException("Country with the same name already exists.");

            country.CountryID = Guid.NewGuid();

            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID is null)
                return null;

            return _countries.FirstOrDefault(country => country.CountryID == countryID)?.ToCountryResponse();
        }
    }
}

using Entities;
using System;
using System.Collections.Generic;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO for adding a new country.
    /// </summary>
    public class CountryAddRequest
    {
        public String? CountryName { get; set; }

        public Country ToCountry()
        {
            return new Country
            {
                CountryName = this.CountryName
            };
        }
    }
}

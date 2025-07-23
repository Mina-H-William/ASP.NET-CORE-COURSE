using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonsSorterService
    {
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

    }
}

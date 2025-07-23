using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonsGetterService
    {
        Task<List<PersonResponse>> GetAllPersons();

        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        Task<MemoryStream> GetPersonsCSVAuto();

        Task<MemoryStream> GetPersonsCSVCustomized();

        Task<MemoryStream> GetPersonsExcel();
    }
}

using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonsAdderService
    {
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
    }
}

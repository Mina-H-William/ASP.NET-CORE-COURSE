using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonsUpdaterService
    {

        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? person_Update);
    }
}

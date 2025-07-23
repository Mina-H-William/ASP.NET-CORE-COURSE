using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonsDeleterService
    {
        Task<bool> DeletePerson(Guid? personID);

    }
}

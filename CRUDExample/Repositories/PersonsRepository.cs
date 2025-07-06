using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RepositoryContracts;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _db.Persons.AddAsync(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid personID)
        {
            Person? person = await _db.Persons.FirstOrDefaultAsync(p => p.PersonID == personID);

            if (person is null)
            {
                return false; // Person not found
            }

            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include(person => person.Country).ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include(person => person.Country).Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonById(Guid id)
        {
            return await _db.Persons.Include(person => person.Country)
                .FirstOrDefaultAsync(p => p.PersonID == id);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(p => p.PersonID == person.PersonID);

            if (matchingPerson == null)
                return person;

            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Gender = person.Gender;
            matchingPerson.CountryID = person.CountryID;
            matchingPerson.Address = person.Address;
            matchingPerson.ReciveNewsLetters = person.ReciveNewsLetters;

            await _db.SaveChangesAsync();

            return person;
        }


    }
}

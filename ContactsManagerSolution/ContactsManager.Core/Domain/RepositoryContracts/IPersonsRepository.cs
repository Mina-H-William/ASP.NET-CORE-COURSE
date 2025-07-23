
using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access operations for persons entity.
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a new person to the system.
        /// </summary>
        /// <param name="person">The person to add. Must not be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added <see cref="Person"/>
        /// object.</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Retrieves a list of all persons.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Person"/>
        /// objects representing all persons. If no persons are found, the list will be empty.</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Retrieves a person by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the person to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the  <see cref="Person"/> object
        /// if found; otherwise, <see langword="null"/>.</returns>
        Task<Person?> GetPersonById(Guid id);

        /// <summary>
        /// Retrieves a list of <see cref="Person"/> objects that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the filter criteria for selecting <see cref="Person"/> objects.
        /// The expression is evaluated against each <see cref="Person"/> in the data source.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Person"/>
        /// objects that match the specified filter criteria. If no matches are found, the returned list will be empty.</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Deletes a person record identified by the specified unique identifier.
        /// </summary>
        /// <param name="personID">The unique identifier of the person to delete. Must not be <see langword="default"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the person
        /// was successfully deleted; otherwise, <see langword="false"/> if the person could not be found or deleted.</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);

        /// <summary>
        /// Updates the details of an existing person.
        /// </summary>
        /// <param name="person">The <see cref="Person"/> object containing the updated details. Must not be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Person"/>
        /// object.</returns>
        Task<Person> UpdatePerson(Person person);
    }
}

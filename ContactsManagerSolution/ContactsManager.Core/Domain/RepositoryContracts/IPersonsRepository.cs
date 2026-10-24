
using ContactsManager.Core.Domain.Entities;
using System.Linq.Expressions;

namespace ContactsManager.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        ///  Adds a Person object to the data store
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Person object after adding it to the data store</returns>
        Task<Person> AddPerson(Person person);
        /// <summary>
        /// Returns all Persons in the data store
        /// </summary>
        /// <returns>All Persons in the table</returns>
        Task<List<Person>> GetAllPersons();
         /// <summary>
         /// Returns a Person object based on the given person id;
         /// otherwise, it return null
         /// </summary>
         /// <param name="personID">person id to search</param>
         /// <returns>Matching Person or null</returns>
        Task<Person?> GetPersonByPersonID(Guid personID);

        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person,bool>> predicate);

        /// <summary>
        /// Deletes a person object based on the given person id
        /// </summary>
        /// <param name="personID">Person ID (guid) to search</param>
        /// <returns>Returns true, if the deletion is successful;
        /// otherwise false</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);
        /// <summary>
        /// Updates a person object (person name and other details) based on the given person id
        /// </summary>
        /// <param name="person">Person objct to update</param>
        /// <returns>Returns the updated person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}

using Entities.DTO;
using Entities.ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a new person into the list of persons 
        /// </summary>
        /// <param name="personAddRequets">Person To add</param>
        /// <returns>returns the same person details, along with newly generated PersonID</returns>
        PersonResponse? AddPerson(PersonAddRequest? personAddRequets);
        /// <summary>
        /// Returns all persons
        /// </summary>
        /// <returns>Returns a list of objects of PersonResponse type</returns>
        List<PersonResponse?> GetAllPersons();
        /// <summary>
        /// Returns the person object based on the given personID
        /// </summary>
        /// <param name="personID">Person id to search</param>
        /// <returns>Matching person object</returns>
        PersonResponse? GetPersonByPersonID(Guid? personID);
        /// <summary>
        /// Returns all person objects that matches with the given search field and search string 
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching persons based on the given search field and search string</returns>
        List<PersonResponse?> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns sorted List of persons
        /// </summary>
        /// <param name="allPersons">represents list of persons to sort</param>
        /// <param name="sortBy">Name of property (key), based on which the presons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Returns sorted persons as PersonResponse list</returns>
        List<PersonResponse?> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Updates the specified person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update, including person id</param>
        /// <returns>Returns the person response object after updating</returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Deletes a person based on the given person id
        /// </summary>
        /// <param name="personID">Person ID to delete</param>
        /// <returns>Reteurns true, if the deletion is successful;Otherwise false</returns>
        bool DeletePerson(Guid? personID);
    }
}

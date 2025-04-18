using System;
using System.Collections.Generic;
using KhuPhoManager.Models;
using KhuPhoManager.Services;

namespace KhuPhoManager.Controllers
{
    /// <summary>
    /// Extended controller for GUI-specific operations for the neighborhood management system
    /// </summary>
    public class GuiNeighborhoodController
    {
        private readonly Neighborhood _neighborhood;
        private readonly FileService _fileService;

        /// <summary>
        /// Creates a new instance of the NeighborhoodController class
        /// </summary>
        public GuiNeighborhoodController()
        {
            _neighborhood = new Neighborhood();
            _fileService = new FileService();
        }

        /// <summary>
        /// Gets all households in the neighborhood
        /// </summary>
        /// <returns>List of households</returns>
        public IReadOnlyList<Household> GetHouseholds()
        {
            return _neighborhood.Households;
        }

        /// <summary>
        /// Gets a household by its house number
        /// </summary>
        /// <param name="houseNumber">The house number to look for</param>
        /// <returns>The household with the specified house number, or null if not found</returns>
        public Household GetHouseholdByNumber(int houseNumber)
        {
            return _neighborhood.GetHouseholdByNumber(houseNumber);
        }

        /// <summary>
        /// Gets the count of households in the neighborhood
        /// </summary>
        /// <returns>Number of households</returns>
        public int GetHouseholdCount()
        {
            return _neighborhood.Households.Count;
        }

        /// <summary>
        /// Gets the total population in the neighborhood
        /// </summary>
        /// <returns>Total number of people</returns>
        public int GetTotalPopulation()
        {
            return _neighborhood.TotalPopulation;
        }

        /// <summary>
        /// Gets the total number of adults in the neighborhood
        /// </summary>
        /// <returns>Number of adults</returns>
        public int GetTotalAdults()
        {
            return _neighborhood.TotalAdults;
        }

        /// <summary>
        /// Gets the total number of children in the neighborhood
        /// </summary>
        /// <returns>Number of children</returns>
        public int GetTotalChildren()
        {
            return _neighborhood.TotalChildren;
        }

        /// <summary>
        /// Adds a household to the neighborhood
        /// </summary>
        /// <param name="household">The household to add</param>
        public void AddHousehold(Household household)
        {
            if (household == null)
                throw new ArgumentNullException(nameof(household));
                
            _neighborhood.AddHousehold(household);
        }

        /// <summary>
        /// Removes a household from the neighborhood
        /// </summary>
        /// <param name="houseNumber">The house number of the household to remove</param>
        /// <returns>True if the household was found and removed, false otherwise</returns>
        public bool RemoveHousehold(int houseNumber)
        {
            return _neighborhood.RemoveHousehold(houseNumber);
        }

        /// <summary>
        /// Finds a person by their ID
        /// </summary>
        /// <param name="id">The ID to search for</param>
        /// <returns>A tuple containing the person and their household, or null if not found</returns>
        public (IPerson person, Household household)? FindPersonById(string id)
        {
            return _neighborhood.FindPersonById(id);
        }

        /// <summary>
        /// Adds a person to a household
        /// </summary>
        /// <param name="houseNumber">The house number</param>
        /// <param name="person">The person to add</param>
        /// <returns>True if the person was added successfully, false otherwise</returns>
        public bool AddPersonToHousehold(int houseNumber, IPerson person)
        {
            return _neighborhood.AddPersonToHousehold(houseNumber, person);
        }

        /// <summary>
        /// Removes a person from a household
        /// </summary>
        /// <param name="houseNumber">The house number</param>
        /// <param name="id">The ID of the person to remove</param>
        /// <returns>True if the person was removed successfully, false otherwise</returns>
        public bool RemovePersonFromHousehold(int houseNumber, string id)
        {
            return _neighborhood.RemovePersonFromHousehold(houseNumber, id);
        }

        /// <summary>
        /// Updates a person's information
        /// </summary>
        /// <param name="houseNumber">The house number</param>
        /// <param name="originalId">The original ID of the person</param>
        /// <param name="updatedPerson">The updated person information</param>
        /// <returns>True if the person was updated successfully, false otherwise</returns>
        public bool EditPersonInformation(int houseNumber, string originalId, IPerson updatedPerson)
        {
            var household = _neighborhood.GetHouseholdByNumber(houseNumber);
            if (household == null || updatedPerson == null)
                return false;

            // Remove the original person
            if (!household.RemoveMemberById(originalId))
                return false;

            // Add the updated person
            household.AddMember(updatedPerson);
            return true;
        }

        /// <summary>
        /// Gets people sorted by age
        /// </summary>
        /// <param name="ascending">True to sort in ascending order, false for descending</param>
        /// <returns>List of people sorted by age</returns>
        public List<(IPerson person, int houseNumber)> GetPeopleSortedByAge(bool ascending = true)
        {
            return _neighborhood.GetPeopleSortedByAge(ascending);
        }

        /// <summary>
        /// Gets households with the most members
        /// </summary>
        /// <returns>List of households with the most members</returns>
        public List<Household> GetHouseholdsWithMostMembers()
        {
            return _neighborhood.GetHouseholdsWithMostMembers();
        }

        /// <summary>
        /// Gets households with the fewest members
        /// </summary>
        /// <returns>List of households with the fewest members</returns>
        public List<Household> GetHouseholdsWithFewestMembers()
        {
            return _neighborhood.GetHouseholdsWithFewestMembers();
        }

        /// <summary>
        /// Reads neighborhood data from a file
        /// </summary>
        /// <param name="filename">The file to read from</param>
        /// <returns>True if the data was read successfully, false otherwise</returns>
        public bool ReadFromFile(string filename)
        {
            try
            {
                var loadedNeighborhood = _fileService.ReadFromFile(filename);
                
                // Transfer households from loaded neighborhood to the current one
                foreach (var household in loadedNeighborhood.Households)
                {
                    try
                    {
                        _neighborhood.AddHousehold(household);
                    }
                    catch (InvalidOperationException)
                    {
                        // Household already exists, skip it
                    }
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Writes neighborhood data to a file
        /// </summary>
        /// <param name="filename">The file to write to</param>
        /// <returns>True if the data was written successfully, false otherwise</returns>
        public bool WriteToFile(string filename)
        {
            try
            {
                _fileService.WriteToFile(_neighborhood, filename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

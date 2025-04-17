using System;
using System.Collections.Generic;
using System.Linq;

namespace KhuPhoManager.Models
{
    /// <summary>
    /// Represents a neighborhood with multiple households
    /// </summary>
    public class Neighborhood
    {
        private List<Household> _households = new List<Household>();
        
        /// <summary>
        /// Gets a read-only list of all households in the neighborhood
        /// </summary>
        public IReadOnlyList<Household> Households => _households.AsReadOnly();
        
        /// <summary>
        /// Creates a new instance of the Neighborhood class
        /// </summary>
        public Neighborhood()
        {
        }

        /// <summary>
        /// Adds a household to the neighborhood
        /// </summary>
        /// <param name="household">The household to add</param>
        public void AddHousehold(Household household)
        {
            if (household == null)
                throw new ArgumentNullException(nameof(household));
                
            // Ensure there's no duplicate house number
            if (_households.Any(h => h.HouseNumber == household.HouseNumber))
                throw new InvalidOperationException($"A household with house number {household.HouseNumber} already exists.");
                
            _households.Add(household);
        }

        /// <summary>
        /// Gets the total number of people in the neighborhood
        /// </summary>
        public int TotalPopulation => _households.Sum(h => h.Members.Count);
        
        /// <summary>
        /// Gets the total number of adults in the neighborhood
        /// </summary>
        public int TotalAdults => _households.Sum(h => h.AdultCount);
        
        /// <summary>
        /// Gets the total number of children in the neighborhood
        /// </summary>
        public int TotalChildren => _households.Sum(h => h.ChildCount);

        /// <summary>
        /// Find a person by their ID (ID card or birth certificate)
        /// </summary>
        /// <param name="id">The ID to search for</param>
        /// <returns>A tuple containing the person and their household, or null if not found</returns>
        public (IPerson person, Household household)? FindPersonById(string id)
        {
            foreach (var household in _households)
            {
                var person = household.FindMemberById(id);
                if (person != null)
                {
                    return (person, household);
                }
            }
            
            return null;
        }

        /// <summary>
        /// Gets a household by its house number
        /// </summary>
        /// <param name="houseNumber">The house number to search for</param>
        /// <returns>The household with the matching house number, or null if not found</returns>
        public Household GetHouseholdByNumber(int houseNumber)
        {
            return _households.FirstOrDefault(h => h.HouseNumber == houseNumber);
        }
        
        /// <summary>
        /// Add a new member to a household
        /// </summary>
        /// <param name="houseNumber">The house number of the household</param>
        /// <param name="person">The person to add</param>
        /// <returns>True if the household was found and the person was added, false otherwise</returns>
        public bool AddPersonToHousehold(int houseNumber, IPerson person)
        {
            var household = GetHouseholdByNumber(houseNumber);
            
            if (household != null && person != null)
            {
                household.AddMember(person);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Find the households with the most members
        /// </summary>
        /// <returns>List of households with the maximum number of members</returns>
        public List<Household> GetHouseholdsWithMostMembers()
        {
            if (_households.Count == 0)
                return new List<Household>();
                
            int maxMembers = _households.Max(h => h.Members.Count);
            return _households.Where(h => h.Members.Count == maxMembers).ToList();
        }
        
        /// <summary>
        /// Find the households with the fewest members
        /// </summary>
        /// <returns>List of households with the minimum number of members</returns>
        public List<Household> GetHouseholdsWithFewestMembers()
        {
            if (_households.Count == 0)
                return new List<Household>();
                
            int minMembers = _households.Min(h => h.Members.Count);
            return _households.Where(h => h.Members.Count == minMembers).ToList();
        }

        /// <summary>
        /// Remove a person from a household
        /// </summary>
        /// <param name="houseNumber">The house number</param>
        /// <param name="id">The ID of the person to remove</param>
        /// <returns>True if the person was found and removed, false otherwise</returns>
        public bool RemovePersonFromHousehold(int houseNumber, string id)
        {
            var household = GetHouseholdByNumber(houseNumber);
            
            if (household != null)
            {
                return household.RemoveMemberById(id);
            }
            
            return false;
        }

        /// <summary>
        /// Remove a household from the neighborhood
        /// </summary>
        /// <param name="houseNumber">The house number of the household to remove</param>
        /// <returns>True if the household was found and removed, false otherwise</returns>
        public bool RemoveHousehold(int houseNumber)
        {
            var household = GetHouseholdByNumber(houseNumber);
            
            if (household != null)
            {
                _households.Remove(household);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Get all people in the neighborhood sorted by age
        /// </summary>
        /// <param name="ascending">True to sort in ascending order, false for descending</param>
        /// <returns>List of people and their house numbers, sorted by age</returns>
        public List<(IPerson person, int houseNumber)> GetPeopleSortedByAge(bool ascending = true)
        {
            List<(IPerson person, int houseNumber)> allPeople = new List<(IPerson, int)>();
            
            foreach (var household in _households)
            {
                foreach (var person in household.Members)
                {
                    allPeople.Add((person, household.HouseNumber));
                }
            }
            
            if (ascending)
            {
                return allPeople.OrderBy(p => p.person.Age).ToList();
            }
            else
            {
                return allPeople.OrderByDescending(p => p.person.Age).ToList();
            }
        }

    }
}

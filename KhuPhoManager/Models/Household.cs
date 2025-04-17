using System;
using System.Collections.Generic;
using System.Linq;

namespace KhuPhoManager.Models
{
    /// <summary>
    /// Represents a household in the neighborhood
    /// </summary>
    public class Household
    {
        /// <summary>
        /// Gets or sets the house number
        /// </summary>
        public int HouseNumber { get; set; }

        private List<IPerson> _members = new List<IPerson>();

        /// <summary>
        /// Gets a read-only list of all members in the household
        /// </summary>
        public IReadOnlyList<IPerson> Members => _members.AsReadOnly();

        /// <summary>
        /// Creates a new instance of the Household class
        /// </summary>
        public Household()
        {
        }

        /// <summary>
        /// Creates a new instance of the Household class with the specified house number
        /// </summary>
        public Household(int houseNumber)
        {
            HouseNumber = houseNumber;
        }

        /// <summary>
        /// Adds a person to the household
        /// </summary>
        /// <param name="person">The person to add</param>
        public void AddMember(IPerson person)
        {
            _members.Add(person);
        }

        /// <summary>
        /// Gets the number of adults in the household
        /// </summary>
        public int AdultCount => _members.Count(m => m.PersonType == "Adult");

        /// <summary>
        /// Gets the number of children in the household
        /// </summary>
        public int ChildCount => _members.Count(m => m.PersonType == "Child");

        /// <summary>
        /// Removes a member from the household by their ID number
        /// </summary>
        /// <param name="id">The ID number of the person to remove</param>
        /// <returns>True if the person was found and removed, false otherwise</returns>
        public bool RemoveMemberById(string id)
        {
            IPerson personToRemove = _members.FirstOrDefault(p => p.IdNumber == id);
            if (personToRemove != null)
            {
                _members.Remove(personToRemove);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Finds a member by their ID number
        /// </summary>
        /// <param name="id">The ID number to search for</param>
        /// <returns>The person with the matching ID, or null if not found</returns>
        public IPerson FindMemberById(string id)
        {
            return _members.FirstOrDefault(p => p.IdNumber == id);
        }
    }
}

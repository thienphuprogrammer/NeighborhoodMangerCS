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
        private int _houseNumber;
        /// <summary>
        /// Gets or sets the house number
        /// </summary>
        public int HouseNumber 
        { 
            get => _houseNumber; 
            set 
            { 
                if (value <= 0)
                    throw new ArgumentException("House number must be a positive integer", nameof(value));
                _houseNumber = value; 
            } 
        }

        /// <summary>
        /// Gets or sets the address of the household
        /// </summary>
        public string Address { get; set; }

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
        /// Creates a new instance of the Household class with the specified house number and address
        /// </summary>
        public Household(int houseNumber, string address)
        {
            HouseNumber = houseNumber;
            Address = address;
        }

        /// <summary>
        /// Adds a person to the household
        /// </summary>
        /// <param name="person">The person to add</param>
        /// <exception cref="ArgumentNullException">Thrown when person is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when a person with the same ID already exists</exception>
        public void AddMember(IPerson person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));
                
            // Check if a person with the same ID already exists
            if (_members.Any(m => m.Id == person.Id))
                throw new InvalidOperationException($"A person with ID {person.Id} already exists in this household.");
                
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
        /// <exception cref="ArgumentNullException">Thrown when id is null or empty</exception>
        public bool RemoveMemberById(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "ID cannot be null or empty");
                
            IPerson personToRemove = _members.FirstOrDefault(p => p.Id == id);
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
        /// <exception cref="ArgumentNullException">Thrown when id is null or empty</exception>
        public IPerson FindMemberById(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "ID cannot be null or empty");
                
            return _members.FirstOrDefault(p => p.Id == id);
        }
        
        /// <summary>
        /// Gets the average age of all members in the household
        /// </summary>
        /// <returns>The average age, or 0 if there are no members</returns>
        public double GetAverageAge()
        {
            if (_members.Count == 0)
                return 0;
                
            return _members.Average(m => m.Age);
        }
        
        /// <summary>
        /// Gets the oldest member in the household
        /// </summary>
        /// <returns>The oldest person, or null if there are no members</returns>
        public IPerson GetOldestMember()
        {
            if (_members.Count == 0)
                return null;
                
            return _members.OrderByDescending(m => m.Age).First();
        }
        
        /// <summary>
        /// Gets the youngest member in the household
        /// </summary>
        /// <returns>The youngest person, or null if there are no members</returns>
        public IPerson GetYoungestMember()
        {
            if (_members.Count == 0)
                return null;
                
            return _members.OrderBy(m => m.Age).First();
        }
    }
}

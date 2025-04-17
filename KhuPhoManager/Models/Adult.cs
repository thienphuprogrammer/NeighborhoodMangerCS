using System;

namespace KhuPhoManager.Models
{
    /// <summary>
    /// Represents an adult person in the neighborhood
    /// </summary>
    public class Adult : IPerson
    {
        /// <summary>
        /// Gets or sets the adult's full name
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Gets or sets the adult's age
        /// </summary>
        public int Age { get; set; }
        
        /// <summary>
        /// Gets or sets the adult's occupation
        /// </summary>
        public string Occupation { get; set; }
        
        /// <summary>
        /// Gets or sets the adult's ID card number
        /// </summary>
        public string IdNumber { get; set; }
        
        /// <summary>
        /// Gets the person type (always "Adult")
        /// </summary>
        public string PersonType => "Adult";

        /// <summary>
        /// Creates a new instance of the Adult class
        /// </summary>
        public Adult()
        {
        }

        /// <summary>
        /// Creates a new instance of the Adult class with the specified properties
        /// </summary>
        public Adult(string fullName, int age, string occupation, string idNumber)
        {
            FullName = fullName;
            Age = age;
            Occupation = occupation;
            IdNumber = idNumber;
        }
    }
}

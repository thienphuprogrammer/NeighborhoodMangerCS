using System;

namespace KhuPhoManager.Models
{
    /// <summary>
    /// Represents an adult person in the neighborhood
    /// </summary>
    public class Adult : IPerson
    {
        private string _id;
        private string _fullName;
        private int _age;
        private string _occupation;
        private string _idNumber;
        
        /// <summary>
        /// Gets or sets the adult's full name
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value is null or empty</exception>
        public string FullName 
        { 
            get => _fullName; 
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Full name cannot be empty", nameof(value));
                _fullName = value; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the adult's age
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when age is less than 18 or unreasonably high</exception>
        public int Age 
        { 
            get => _age; 
            set 
            { 
                if (value < 18)
                    throw new ArgumentException("Adult age must be at least 18", nameof(value));
                if (value > 120)
                    throw new ArgumentException("Age is unreasonably high", nameof(value));
                _age = value; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the adult's occupation
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value is null or empty</exception>
        public string Occupation 
        { 
            get => _occupation; 
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Occupation cannot be empty", nameof(value));
                _occupation = value; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the adult's ID card number
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value is null or empty</exception>
        public string IdNumber 
        { 
            get => _idNumber; 
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("ID number cannot be empty", nameof(value));
                _idNumber = value; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the person's unique identifier
        /// </summary>
        public string Id 
        { 
            get => _id; 
            set => _id = value ?? Guid.NewGuid().ToString(); 
        }

        /// <summary>
        /// Gets the person type (always "Adult")
        /// </summary>
        public string PersonType => "Adult";

        /// <summary>
        /// Creates a new instance of the Adult class
        /// </summary>
        public Adult()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates a new instance of the Adult class with the specified properties
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
        public Adult(string fullName, int age, string occupation, string idNumber)
        {
            // These property setters will validate the input
            Id = Guid.NewGuid().ToString();
            FullName = fullName;
            Age = age;
            Occupation = occupation;
            IdNumber = idNumber;
        }

        /// <summary>
        /// Creates a new instance of the Adult class with the specified properties (without ID number)
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
        public Adult(string fullName, int age, string occupation)
        {
            // These property setters will validate the input
            Id = Guid.NewGuid().ToString();
            FullName = fullName;
            Age = age;
            Occupation = occupation;
            IdNumber = Guid.NewGuid().ToString().Substring(0, 12); // Generate a placeholder ID
        }
        
        /// <summary>
        /// Returns a string representation of the adult
        /// </summary>
        /// <returns>A string containing the adult's details</returns>
        public override string ToString()
        {
            return $"Adult: {FullName}, Age: {Age}, Occupation: {Occupation}, ID: {IdNumber}";
        }
    }
}

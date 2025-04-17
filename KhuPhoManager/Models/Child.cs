using System;

namespace KhuPhoManager.Models
{
    /// <summary>
    /// Represents a child in the neighborhood
    /// </summary>
    public class Child : IPerson
    {
        /// <summary>
        /// Gets or sets the child's full name
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Gets or sets the child's age
        /// </summary>
        public int Age { get; set; }
        
        /// <summary>
        /// Gets or sets the school class the child attends
        /// </summary>
        public string SchoolClass { get; set; }
        
        /// <summary>
        /// Gets or sets the child's birth certificate number
        /// </summary>
        public string BirthCertificateNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the child's occupation (always "Student")
        /// </summary>
        public string Occupation { get => "Student"; set { /* Not used */ } }
        
        /// <summary>
        /// Gets or sets the child's ID (maps to birth certificate number)
        /// </summary>
        public string IdNumber 
        { 
            get => BirthCertificateNumber; 
            set => BirthCertificateNumber = value; 
        }
        
        /// <summary>
        /// Gets the person type (always "Child")
        /// </summary>
        public string PersonType => "Child";

        /// <summary>
        /// Creates a new instance of the Child class
        /// </summary>
        public Child()
        {
        }

        /// <summary>
        /// Creates a new instance of the Child class with the specified properties
        /// </summary>
        public Child(string fullName, int age, string schoolClass, string birthCertificateNumber)
        {
            FullName = fullName;
            Age = age;
            SchoolClass = schoolClass;
            BirthCertificateNumber = birthCertificateNumber;
        }
    }
}

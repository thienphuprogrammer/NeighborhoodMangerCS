using System;

namespace KhuPhoManager.Models
{
    /// <summary>
    /// Represents a child in the neighborhood
    /// </summary>
    public class Child : IPerson
    {
        private string _id;
        private string _fullName;
        private int _age;
        private string _schoolClass;
        private string _birthCertificateNumber;
        private string _school;
        private int _grade;
        
        /// <summary>
        /// Gets or sets the child's full name
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
        /// Gets or sets the child's age
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when age is negative or greater than 17</exception>
        public int Age 
        { 
            get => _age; 
            set 
            { 
                if (value < 0)
                    throw new ArgumentException("Age cannot be negative", nameof(value));
                if (value >= 18)
                    throw new ArgumentException("Child age must be less than 18", nameof(value));
                _age = value; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the school class the child attends
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value is null or empty</exception>
        public string SchoolClass 
        { 
            get => _schoolClass; 
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("School class cannot be empty", nameof(value));
                _schoolClass = value; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the child's birth certificate number
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value is null or empty</exception>
        public string BirthCertificateNumber 
        { 
            get => _birthCertificateNumber; 
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Birth certificate number cannot be empty", nameof(value));
                _birthCertificateNumber = value; 
            } 
        }
        
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
        /// Gets or sets the child's unique identifier
        /// </summary>
        public string Id 
        { 
            get => _id; 
            set => _id = value ?? Guid.NewGuid().ToString(); 
        }

        /// <summary>
        /// Gets or sets the school name
        /// </summary>
        public string School
        {
            get => _school;
            set => _school = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the grade level
        /// </summary>
        public int Grade
        {
            get => _grade;
            set => _grade = value;
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
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates a new instance of the Child class with the specified properties
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
        public Child(string fullName, int age, string schoolClass, string birthCertificateNumber)
        {
            // These property setters will validate the input
            Id = Guid.NewGuid().ToString();
            FullName = fullName;
            Age = age;
            SchoolClass = schoolClass;
            BirthCertificateNumber = birthCertificateNumber;
        }

        /// <summary>
        /// Creates a new instance of the Child class with school and grade information
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
        public Child(string fullName, int age, string school, int grade, string birthCertificateNumber)
        {
            // These property setters will validate the input
            Id = Guid.NewGuid().ToString();
            FullName = fullName;
            Age = age;
            School = school;
            Grade = grade;
            SchoolClass = $"Grade {grade}";
            BirthCertificateNumber = birthCertificateNumber;
        }
        
        /// <summary>
        /// Returns a string representation of the child
        /// </summary>
        /// <returns>A string containing the child's details</returns>
        public override string ToString()
        {
            return $"Child: {FullName}, Age: {Age}, School Class: {SchoolClass}, Birth Certificate: {BirthCertificateNumber}";
        }
        
        /// <summary>
        /// Determines the education level based on the child's age
        /// </summary>
        /// <returns>The education level (Preschool, Elementary, Middle, High)</returns>
        public string GetEducationLevel()
        {
            if (Age < 5)
                return "Preschool";
            else if (Age < 11)
                return "Elementary School";
            else if (Age < 15)
                return "Middle School";
            else
                return "High School";
        }
    }
}

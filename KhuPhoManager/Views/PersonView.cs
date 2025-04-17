using System;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views
{
    /// <summary>
    /// View for person-related UI operations
    /// </summary>
    public class PersonView
    {
        /// <summary>
        /// Displays information about a person
        /// </summary>
        /// <param name="person">The person to display</param>
        public void DisplayPerson(IPerson person)
        {
            if (person is Adult adult)
            {
                Console.WriteLine($" - [Adult] Full name: {adult.FullName}, Age: {adult.Age}, Occupation: {adult.Occupation}, ID: {adult.IdNumber}");
            }
            else if (person is Child child)
            {
                Console.WriteLine($" - [Child] Full name: {child.FullName}, Age: {child.Age}, School Class: {child.SchoolClass}, Birth Certificate: {child.BirthCertificateNumber}");
            }
        }

        /// <summary>
        /// Gets input for an adult
        /// </summary>
        /// <returns>A new Adult object with user-provided data</returns>
        public Adult InputAdult()
        {
            Adult adult = new Adult();
            
            Console.Write("Enter full name: ");
            adult.FullName = Console.ReadLine();
            
            Console.Write("Enter age: ");
            int age;
            while (!int.TryParse(Console.ReadLine(), out age) || age < 0)
            {
                Console.Write("Invalid age. Please enter a valid age: ");
            }
            adult.Age = age;
            
            Console.Write("Enter occupation: ");
            adult.Occupation = Console.ReadLine();
            
            Console.Write("Enter ID number: ");
            adult.IdNumber = Console.ReadLine();
            
            return adult;
        }

        /// <summary>
        /// Gets input for a child
        /// </summary>
        /// <returns>A new Child object with user-provided data</returns>
        public Child InputChild()
        {
            Child child = new Child();
            
            Console.Write("Enter child's full name: ");
            child.FullName = Console.ReadLine();
            
            Console.Write("Enter age: ");
            int age;
            while (!int.TryParse(Console.ReadLine(), out age) || age < 0)
            {
                Console.Write("Invalid age. Please enter a valid age: ");
            }
            child.Age = age;
            
            Console.Write("Enter school class: ");
            child.SchoolClass = Console.ReadLine();
            
            Console.Write("Enter birth certificate number: ");
            child.BirthCertificateNumber = Console.ReadLine();
            
            return child;
        }

        /// <summary>
        /// Gets input for person type selection
        /// </summary>
        /// <returns>1 for Adult, 2 for Child</returns>
        public int GetPersonTypeChoice()
        {
            Console.WriteLine("\n1. Adult\n2. Child\nChoose (1-2): ");
            
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 2)
            {
                Console.Write("Invalid choice. Please choose 1 or 2: ");
            }
            
            return choice;
        }

        /// <summary>
        /// Creates a new person based on user input
        /// </summary>
        /// <returns>A new IPerson object (either Adult or Child)</returns>
        public IPerson InputPerson()
        {
            int choice = GetPersonTypeChoice();
            return choice == 1 ? InputAdult() : (IPerson)InputChild();
        }

        /// <summary>
        /// Displays people sorted by age
        /// </summary>
        /// <param name="sortedPeople">List of persons and their house numbers, sorted by age</param>
        public void DisplayPeopleSorted(System.Collections.Generic.List<(IPerson person, int houseNumber)> sortedPeople)
        {
            if (sortedPeople == null)
            {
                Console.WriteLine("No people to display.");
                return;
            }

            Console.WriteLine("\n===== PEOPLE SORTED BY AGE =====");
            
            if (sortedPeople.Count == 0)
            {
                Console.WriteLine("No people in the neighborhood.");
                return;
            }

            foreach (var (person, houseNumber) in sortedPeople)
            {
                if (person != null)
                {
                    Console.Write($"House Number: {houseNumber} - ");
                    DisplayPerson(person);
                }
            }
        }
    }
}

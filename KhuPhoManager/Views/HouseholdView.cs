using System;
using System.Collections.Generic;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views
{
    /// <summary>
    /// View for household-related UI operations
    /// </summary>
    public class HouseholdView
    {
        private readonly PersonView _personView;

        /// <summary>
        /// Creates a new instance of the HouseholdView class
        /// </summary>
        public HouseholdView()
        {
            _personView = new PersonView();
        }

        /// <summary>
        /// Displays information about a household
        /// </summary>
        /// <param name="household">The household to display</param>
        public void DisplayHousehold(Household household)
        {
            Console.WriteLine($"\nHouse Number: {household.HouseNumber}");
            Console.WriteLine($"Number of members: {household.Members.Count}");
            
            foreach (var member in household.Members)
            {
                _personView.DisplayPerson(member);
            }
        }

        /// <summary>
        /// Gets input for a new household
        /// </summary>
        /// <returns>A new Household object with user-provided data</returns>
        public Household InputHousehold()
        {
            Console.Write("\nEnter house number: ");
            int houseNumber;
            while (!int.TryParse(Console.ReadLine(), out houseNumber) || houseNumber <= 0)
            {
                Console.Write("Invalid house number. Please enter a positive number: ");
            }

            Household household = new Household(houseNumber);

            Console.Write("Enter number of members: ");
            int memberCount;
            while (!int.TryParse(Console.ReadLine(), out memberCount) || memberCount < 0)
            {
                Console.Write("Invalid number of members. Please enter a non-negative number: ");
            }

            for (int i = 0; i < memberCount; i++)
            {
                Console.WriteLine($"\nMember {i + 1}:");
                IPerson person = _personView.InputPerson();
                household.AddMember(person);
            }

            return household;
        }

        /// <summary>
        /// Displays statistics for a household
        /// </summary>
        /// <param name="household">The household to analyze</param>
        public void DisplayHouseholdStatistics(Household household)
        {
            Console.WriteLine($"\nStatistics for House Number {household.HouseNumber}:");
            Console.WriteLine($"Total members: {household.Members.Count}");
            Console.WriteLine($"Adults: {household.AdultCount}");
            Console.WriteLine($"Children: {household.ChildCount}");
        }

        /// <summary>
        /// Displays a list of households with most members
        /// </summary>
        /// <param name="households">The list of households</param>
        public void DisplayHouseholdsWithMostMembers(List<Household> households)
        {
            if (households.Count == 0)
            {
                Console.WriteLine("No households in the neighborhood.");
                return;
            }
            
            int maxMembers = households[0].Members.Count;
            
            Console.WriteLine("\n===== HOUSEHOLDS WITH MOST MEMBERS =====");
            Console.WriteLine($"Maximum number of members: {maxMembers}");
            foreach (var household in households)
            {
                Console.WriteLine($"  House Number: {household.HouseNumber}");
            }
        }

        /// <summary>
        /// Displays a list of households with fewest members
        /// </summary>
        /// <param name="households">The list of households</param>
        public void DisplayHouseholdsWithFewestMembers(List<Household> households)
        {
            if (households.Count == 0)
            {
                Console.WriteLine("No households in the neighborhood.");
                return;
            }
            
            int minMembers = households[0].Members.Count;
            
            Console.WriteLine("\n===== HOUSEHOLDS WITH FEWEST MEMBERS =====");
            Console.WriteLine($"Minimum number of members: {minMembers}");
            foreach (var household in households)
            {
                Console.WriteLine($"  House Number: {household.HouseNumber}");
            }
        }
    }
}

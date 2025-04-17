using System;
using System.Collections.Generic;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views
{
    /// <summary>
    /// View for neighborhood-related UI operations
    /// </summary>
    public class NeighborhoodView
    {
        private readonly HouseholdView _householdView;
        private readonly PersonView _personView;

        /// <summary>
        /// Creates a new instance of the NeighborhoodView class
        /// </summary>
        public NeighborhoodView()
        {
            _householdView = new HouseholdView();
            _personView = new PersonView();
        }

        /// <summary>
        /// Displays the neighborhood information
        /// </summary>
        /// <param name="neighborhood">The neighborhood to display</param>
        public void DisplayNeighborhood(Neighborhood neighborhood)
        {
            Console.WriteLine("\n===== NEIGHBORHOOD INFORMATION =====");
            Console.WriteLine($"Number of households: {neighborhood.Households.Count}");
            
            foreach (var household in neighborhood.Households)
            {
                _householdView.DisplayHousehold(household);
            }
        }

        /// <summary>
        /// Gets input for household count
        /// </summary>
        /// <returns>The number of households to input</returns>
        public int GetHouseholdCount()
        {
            Console.Write("Enter number of households in the neighborhood: ");
            int houseCount;
            while (!int.TryParse(Console.ReadLine(), out houseCount) || houseCount < 0)
            {
                Console.Write("Invalid number. Please enter a non-negative number: ");
            }
            
            return houseCount;
        }

        /// <summary>
        /// Gets a house number from the user
        /// </summary>
        /// <returns>The house number</returns>
        public int GetHouseNumber()
        {
            Console.Write("Enter house number: ");
            int houseNumber;
            while (!int.TryParse(Console.ReadLine(), out houseNumber) || houseNumber <= 0)
            {
                Console.Write("Invalid house number. Please enter a positive number: ");
            }
            
            return houseNumber;
        }

        /// <summary>
        /// Gets ID number from the user
        /// </summary>
        /// <returns>The ID number</returns>
        public string GetIdNumber()
        {
            Console.Write("Enter ID/birth certificate number: ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Gets filename from the user
        /// </summary>
        /// <param name="defaultFilename">Default filename to use if none provided</param>
        /// <returns>The filename</returns>
        public string GetFilename(string defaultFilename = "Danhsach.csv")
        {
            Console.Write($"Enter filename (default: {defaultFilename}): ");
            string filename = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(filename))
            {
                return defaultFilename;
            }
            
            return filename;
        }

        /// <summary>
        /// Gets sort direction from the user
        /// </summary>
        /// <returns>True for ascending, false for descending</returns>
        public bool GetSortDirection()
        {
            Console.WriteLine("1. Sort by ascending age\n2. Sort by descending age");
            Console.Write("Choose: ");
            
            int sortChoice;
            while (!int.TryParse(Console.ReadLine(), out sortChoice) || sortChoice < 1 || sortChoice > 2)
            {
                Console.Write("Invalid choice. Please choose 1 or 2: ");
            }
            
            return sortChoice == 1;
        }

        /// <summary>
        /// Displays neighborhood statistics
        /// </summary>
        /// <param name="neighborhood">The neighborhood to analyze</param>
        public void DisplayNeighborhoodStatistics(Neighborhood neighborhood)
        {
            Console.WriteLine("\n===== NEIGHBORHOOD STATISTICS =====");
            Console.WriteLine($"Total households: {neighborhood.Households.Count}");
            Console.WriteLine($"Total residents: {neighborhood.TotalPopulation}");
            Console.WriteLine($"Adults: {neighborhood.TotalAdults}");
            Console.WriteLine($"Children: {neighborhood.TotalChildren}");
        }

        /// <summary>
        /// Displays the main menu and gets the user's selection
        /// </summary>
        /// <returns>The menu choice (0-14)</returns>
        public int DisplayMenuAndGetChoice()
        {
            Console.WriteLine("\n===== MENU =====");
            Console.WriteLine("1. Input neighborhood data");
            Console.WriteLine("2. Display neighborhood data");
            Console.WriteLine("3. Find person by ID");
            Console.WriteLine("4. Add person to household");
            Console.WriteLine("5. Neighborhood statistics");
            Console.WriteLine("6. Household statistics");
            Console.WriteLine("7. Edit person information");
            Console.WriteLine("8. Read data from file");
            Console.WriteLine("9. Write data to file");
            Console.WriteLine("10. Add new household");
            Console.WriteLine("11. Remove person from household");
            Console.WriteLine("12. Remove household");
            Console.WriteLine("13. Sort by age");
            Console.WriteLine("14. Find households with most/fewest members");
            Console.WriteLine("0. Exit");
            Console.Write("Choose: ");

            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 14)
            {
                Console.Write("Invalid choice. Please enter a number between 0 and 14: ");
            }
            
            return choice;
        }

        /// <summary>
        /// Displays a success message
        /// </summary>
        /// <param name="message">The message to display</param>
        public void DisplaySuccess(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message to display</param>
        public void DisplayError(string message)
        {
            Console.WriteLine(message);
        }
    }
}

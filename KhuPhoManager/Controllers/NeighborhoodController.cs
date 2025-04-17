using System;
using KhuPhoManager.Models;
using KhuPhoManager.Views;
using KhuPhoManager.Services;

namespace KhuPhoManager.Controllers
{
    /// <summary>
    /// Controller for neighborhood-related operations
    /// </summary>
    public class NeighborhoodController
    {
        private readonly Neighborhood _neighborhood;
        private readonly NeighborhoodView _neighborhoodView;
        private readonly HouseholdView _householdView;
        private readonly PersonView _personView;
        private readonly FileService _fileService;

        /// <summary>
        /// Creates a new instance of the NeighborhoodController class
        /// </summary>
        public NeighborhoodController()
        {
            _neighborhood = new Neighborhood();
            _neighborhoodView = new NeighborhoodView();
            _householdView = new HouseholdView();
            _personView = new PersonView();
            _fileService = new FileService();
        }

        /// <summary>
        /// Inputs neighborhood data
        /// </summary>
        public void InputNeighborhood()
        {
            int houseCount = _neighborhoodView.GetHouseholdCount();
            
            for (int i = 0; i < houseCount; i++)
            {
                Console.WriteLine($"\nHousehold {i + 1}:");
                try
                {
                    Household household = _householdView.InputHousehold();
                    _neighborhood.AddHousehold(household);
                }
                catch (InvalidOperationException ex)
                {
                    _neighborhoodView.DisplayError(ex.Message);
                    i--; // Retry this household
                }
            }
        }

        /// <summary>
        /// Displays neighborhood data
        /// </summary>
        public void DisplayNeighborhood()
        {
            _neighborhoodView.DisplayNeighborhood(_neighborhood);
        }

        /// <summary>
        /// Finds a person by ID
        /// </summary>
        public void FindPersonById()
        {
            string id = _neighborhoodView.GetIdNumber();
            var result = _neighborhood.FindPersonById(id);
            
            if (result.HasValue)
            {
                var (person, household) = result.Value;
                Console.WriteLine($"\nFound in House Number: {household.HouseNumber}");
                _personView.DisplayPerson(person);
            }
            else
            {
                _neighborhoodView.DisplayError($"\nNo person found with ID: {id}");
            }
        }

        /// <summary>
        /// Adds a person to a household
        /// </summary>
        public void AddPersonToHousehold()
        {
            int houseNumber = _neighborhoodView.GetHouseNumber();
            var household = _neighborhood.GetHouseholdByNumber(houseNumber);
            
            if (household != null)
            {
                IPerson person = _personView.InputPerson();
                household.AddMember(person);
                _neighborhoodView.DisplaySuccess($"Person added to household with house number {houseNumber}.");
            }
            else
            {
                _neighborhoodView.DisplayError($"No household found with house number {houseNumber}.");
            }
        }

        /// <summary>
        /// Displays neighborhood statistics
        /// </summary>
        public void DisplayNeighborhoodStatistics()
        {
            _neighborhoodView.DisplayNeighborhoodStatistics(_neighborhood);
        }

        /// <summary>
        /// Displays statistics for a specific household
        /// </summary>
        public void DisplayHouseholdStatistics()
        {
            int houseNumber = _neighborhoodView.GetHouseNumber();
            var household = _neighborhood.GetHouseholdByNumber(houseNumber);
            
            if (household != null)
            {
                _householdView.DisplayHouseholdStatistics(household);
            }
            else
            {
                _neighborhoodView.DisplayError($"No household found with house number {houseNumber}.");
            }
        }

        /// <summary>
        /// Edits person information
        /// </summary>
        public void EditPersonInformation()
        {
            string id = _neighborhoodView.GetIdNumber();
            var result = _neighborhood.FindPersonById(id);
            
            if (result.HasValue)
            {
                var (person, household) = result.Value;
                Console.WriteLine($"Found in House Number: {household.HouseNumber}");
                _personView.DisplayPerson(person);
                
                Console.WriteLine("\nEnter new information:");
                IPerson newPerson;
                
                if (person is Adult)
                {
                    newPerson = _personView.InputAdult();
                }
                else
                {
                    newPerson = _personView.InputChild();
                }
                
                // Replace the old person with the new one
                if (household.RemoveMemberById(id))
                {
                    household.AddMember(newPerson);
                    _neighborhoodView.DisplaySuccess("Information updated successfully.");
                }
            }
            else
            {
                _neighborhoodView.DisplayError($"No person found with ID: {id}");
            }
        }

        /// <summary>
        /// Reads data from a file
        /// </summary>
        public void ReadFromFile()
        {
            string filename = _neighborhoodView.GetFilename();
            
            try
            {
                var loadedNeighborhood = _fileService.ReadFromFile(filename);
                
                // Transfer households from loaded neighborhood to the current one
                foreach (var household in loadedNeighborhood.Households)
                {
                    try
                    {
                        _neighborhood.AddHousehold(household);
                    }
                    catch (InvalidOperationException)
                    {
                        // Household already exists, skip it
                    }
                }
                
                _neighborhoodView.DisplaySuccess($"Successfully read data from {filename}.");
                _neighborhoodView.DisplaySuccess($"Loaded {loadedNeighborhood.Households.Count} households with {loadedNeighborhood.TotalPopulation} members.");
            }
            catch (Exception ex)
            {
                _neighborhoodView.DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Writes data to a file
        /// </summary>
        public void WriteToFile()
        {
            string filename = _neighborhoodView.GetFilename();
            
            try
            {
                _fileService.WriteToFile(_neighborhood, filename);
                _neighborhoodView.DisplaySuccess($"Successfully wrote data to {filename}.");
            }
            catch (Exception ex)
            {
                _neighborhoodView.DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Adds a new household
        /// </summary>
        public void AddHousehold()
        {
            try
            {
                Household household = _householdView.InputHousehold();
                _neighborhood.AddHousehold(household);
                _neighborhoodView.DisplaySuccess("Household added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                _neighborhoodView.DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Removes a person from a household
        /// </summary>
        public void RemovePersonFromHousehold()
        {
            int houseNumber = _neighborhoodView.GetHouseNumber();
            string id = _neighborhoodView.GetIdNumber();
            
            bool success = _neighborhood.RemovePersonFromHousehold(houseNumber, id);
            
            if (success)
            {
                _neighborhoodView.DisplaySuccess($"Person with ID {id} removed from household with house number {houseNumber}.");
            }
            else
            {
                _neighborhoodView.DisplayError($"Failed to remove person with ID {id} from household with house number {houseNumber}.");
            }
        }

        /// <summary>
        /// Removes a household
        /// </summary>
        public void RemoveHousehold()
        {
            int houseNumber = _neighborhoodView.GetHouseNumber();
            
            bool success = _neighborhood.RemoveHousehold(houseNumber);
            
            if (success)
            {
                _neighborhoodView.DisplaySuccess($"Household with house number {houseNumber} removed successfully.");
            }
            else
            {
                _neighborhoodView.DisplayError($"No household found with house number {houseNumber}.");
            }
        }

        /// <summary>
        /// Sorts people by age
        /// </summary>
        public void SortByAge()
        {
            bool ascending = _neighborhoodView.GetSortDirection();
            var sortedPeople = _neighborhood.GetPeopleSortedByAge(ascending);
            _personView.DisplayPeopleSorted(sortedPeople);
        }

        /// <summary>
        /// Finds households with most/fewest members
        /// </summary>
        public void FindHouseholdsWithMaxMinMembers()
        {
            var householdsWithMostMembers = _neighborhood.GetHouseholdsWithMostMembers();
            var householdsWithFewestMembers = _neighborhood.GetHouseholdsWithFewestMembers();
            
            _householdView.DisplayHouseholdsWithMostMembers(householdsWithMostMembers);
            _householdView.DisplayHouseholdsWithFewestMembers(householdsWithFewestMembers);
        }

        /// <summary>
        /// Runs the main application loop
        /// </summary>
        public void Run()
        {
            int choice;
            
            do
            {
                choice = _neighborhoodView.DisplayMenuAndGetChoice();
                
                try
                {
                    switch (choice)
                    {
                        case 1: InputNeighborhood(); break;
                        case 2: DisplayNeighborhood(); break;
                        case 3: FindPersonById(); break;
                        case 4: AddPersonToHousehold(); break;
                        case 5: DisplayNeighborhoodStatistics(); break;
                        case 6: DisplayHouseholdStatistics(); break;
                        case 7: EditPersonInformation(); break;
                        case 8: ReadFromFile(); break;
                        case 9: WriteToFile(); break;
                        case 10: AddHousehold(); break;
                        case 11: RemovePersonFromHousehold(); break;
                        case 12: RemoveHousehold(); break;
                        case 13: SortByAge(); break;
                        case 14: FindHouseholdsWithMaxMinMembers(); break;
                        case 0: _neighborhoodView.DisplaySuccess("Exiting program."); break;
                        default: _neighborhoodView.DisplayError("Invalid choice!"); break;
                    }
                }
                catch (Exception ex)
                {
                    _neighborhoodView.DisplayError($"An error occurred: {ex.Message}");
                }
            } while (choice != 0);
        }
    }
}

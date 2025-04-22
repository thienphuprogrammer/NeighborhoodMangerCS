using System;
using System.Collections.Generic;
using System.IO;
using KhuPhoManager.Models;
using System.Globalization;

namespace KhuPhoManager.Services
{
    /// <summary>
    /// Service for handling file operations
    /// </summary>
    public class FileService
    {
        /// <summary>
        /// Reads neighborhood data from a CSV file
        /// </summary>
        /// <param name="filename">Path to the CSV file</param>
        /// <returns>A Neighborhood object populated with data from the file</returns>
        public Neighborhood ReadFromFile(string filename)
        {
            Neighborhood neighborhood = new Neighborhood();
            
            try
            {
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException($"File {filename} not found.");
                }

                Dictionary<int, Household> householdMap = new Dictionary<int, Household>();

                string[] lines = File.ReadAllLines(filename);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length < 6) continue; // Skip invalid lines

                    int houseNumber = int.Parse(parts[0]);
                    string address = parts[1];
                    string personType = parts[2];
                    string fullName = parts[3];
                    int age = int.Parse(parts[4]);
                    string OccupationOrSchool = parts[5];
                    string idNumber = parts.Length > 6 ? parts[6] : string.Empty;
                    DateTime dateOfBirth = DateTime.ParseExact(parts[7], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    int grade = parts.Length > 8 ? int.Parse(parts[8]) : 0;

                    // Create or get household
                    if (!householdMap.TryGetValue(houseNumber, out Household household))
                    {
                        household = new Household(houseNumber, address);
                        householdMap[houseNumber] = household;
                        
                        try
                        {
                            neighborhood.AddHousehold(household);
                        }
                        catch (InvalidOperationException)
                        {
                            // Household with this number already exists, just use it
                            household = neighborhood.GetHouseholdByNumber(houseNumber);
                        }
                    }

                    // Add person to household
                    IPerson person;
                    if (personType.Equals("Adult", StringComparison.OrdinalIgnoreCase))
                    {
                        person = new Adult
                        {
                            FullName = fullName,
                            Age = age,
                            Occupation = OccupationOrSchool,
                            IdNumber = idNumber,
                            DateOfBirth = dateOfBirth
                        };
                    }
                    else
                    {
                        person = new Child
                        {
                            FullName = fullName,
                            Age = age,
                            School = OccupationOrSchool,
                            IdNumber = idNumber,
                            Grade = grade,
                            DateOfBirth = dateOfBirth
                        };
                    }
                    
                    household.AddMember(person);
                }
                
                return neighborhood;
            }
            catch (Exception ex)
            {
                throw new IOException($"Error reading from file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Writes neighborhood data to a CSV file
        /// </summary>
        /// <param name="neighborhood">The neighborhood data to write</param>
        /// <param name="filename">Path to the CSV file</param>
        public void WriteToFile(Neighborhood neighborhood, string filename)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    foreach (var household in neighborhood.Households)
                    {
                        foreach (var person in household.Members)
                        {
                            string line;
                            if (person is Adult adult)
                            {
                                line = $"{household.HouseNumber},{household.Address},Adult,{adult.FullName},{adult.Age},{adult.Occupation},{adult.IdNumber},{adult.DateOfBirth}";
                            }
                            else if (person is Child child)
                            {
                                line = $"{household.HouseNumber},{household.Address},Child,{child.FullName},{child.Age},{child.School},{child.IdNumber},{child.DateOfBirth},{child.Grade}";
                            }
                            else
                            {
                                continue; // Skip unknown person types
                            }
                            
                            writer.WriteLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Error writing to file: {ex.Message}", ex);
            }
        }
    }
}

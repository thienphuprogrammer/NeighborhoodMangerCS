using System;
using System.Collections.Generic;
using System.IO;
using KhuPhoManager.Models;

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
                    if (parts.Length < 5) continue; // Skip invalid lines

                    int houseNumber = int.Parse(parts[0]);
                    string personType = parts[1];
                    string fullName = parts[2];
                    int age = int.Parse(parts[3]);
                    string idOrClass = parts[4];
                    string idNumber = parts.Length > 5 ? parts[5] : string.Empty;

                    // Create or get household
                    if (!householdMap.TryGetValue(houseNumber, out Household household))
                    {
                        household = new Household(houseNumber);
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
                            Occupation = idOrClass,
                            IdNumber = idNumber
                        };
                    }
                    else
                    {
                        person = new Child
                        {
                            FullName = fullName,
                            Age = age,
                            SchoolClass = idOrClass,
                            BirthCertificateNumber = idNumber
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
                                line = $"{household.HouseNumber},Adult,{adult.FullName},{adult.Age},{adult.Occupation},{adult.IdNumber}";
                            }
                            else if (person is Child child)
                            {
                                line = $"{household.HouseNumber},Child,{child.FullName},{child.Age},{child.SchoolClass},{child.BirthCertificateNumber}";
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

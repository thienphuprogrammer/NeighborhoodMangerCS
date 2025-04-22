using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using KhuPhoManager.Models;

namespace KhuPhoManager.Services
{
    public class FileService
    {
        // Helper: Escape CSV fields
        private string EscapeCsv(string field)
        {
            if (field.Contains("\""))
                field = field.Replace("\"", "\"\"");
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
                field = $"\"{field}\"";
            return field;
        }

        // Helper: Parse CSV line into fields
        private List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            var sb = new StringBuilder();
            foreach (char c in line)
            {
                if (c == '\"')
                {
                    if (inQuotes && sb.Length > 0 && sb[sb.Length - 1] == '\"')
                        sb.Append('\"'); // Escaped quote
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            fields.Add(sb.ToString());
            return fields;
        }

        public Neighborhood ReadFromFile(string filename)
        {
            Neighborhood neighborhood = new Neighborhood();
            try
            {
                if (!File.Exists(filename))
                    throw new FileNotFoundException($"File {filename} not found.");

                Dictionary<int, Household> householdMap = new Dictionary<int, Household>();
                string[] lines = File.ReadAllLines(filename);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = ParseCsvLine(line);
                    if (parts.Count < 8) continue; // Minimum for Adult

                    int houseNumber = int.Parse(parts[0]);
                    string address = parts[1];
                    string id = parts[2];
                    string personType = parts[3];
                    string fullName = parts[4];
                    int age = int.Parse(parts[5]);
                    string occupationOrSchool = parts[6];
                    string idNumberOrBirthCertificateNumber = parts[7];
                    DateTime dateOfBirth = DateTime.MinValue;
                    int grade = 0;

                    if (parts.Count > 8)
                        DateTime.TryParse(parts[8], out dateOfBirth);
                    if (parts.Count > 9)
                        int.TryParse(parts[9], out grade);

                    if (!householdMap.TryGetValue(houseNumber, out Household household))
                    {
                        household = new Household(houseNumber, address);
                        householdMap[houseNumber] = household;
                        try { neighborhood.AddHousehold(household); }
                        catch { household = neighborhood.GetHouseholdByNumber(houseNumber); }
                    }

                    IPerson person;
                    if (personType.Equals("Adult", StringComparison.OrdinalIgnoreCase))
                    {
                        person = new Adult
                        {
                            FullName = fullName,
                            Age = age,
                            Occupation = occupationOrSchool,
                            Id = id,
                            IdNumber = idNumberOrBirthCertificateNumber,
                            DateOfBirth = dateOfBirth
                        };
                    }
                    else
                    {
                        person = new Child
                        {
                            FullName = fullName,
                            Age = age,
                            School = occupationOrSchool,
                            Grade = grade,
                            Id = id,
                            DateOfBirth = dateOfBirth,
                            BirthCertificateNumber = idNumberOrBirthCertificateNumber
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

        public void WriteToFile(Neighborhood neighborhood, string filename)
        {
            try
            {
                var sb = new StringBuilder();
                foreach (var household in neighborhood.Households)
                {
                    foreach (var person in household.Members)
                    {
                        string line;
                        if (person is Adult adult)
                        {
                            line = string.Join(",",
                                EscapeCsv(household.HouseNumber.ToString()),
                                EscapeCsv(household.Address),
                                EscapeCsv(adult.Id),
                                "Adult",
                                EscapeCsv(adult.FullName),
                                adult.Age,
                                EscapeCsv(adult.Occupation),
                                EscapeCsv(adult.IdNumber),
                                EscapeCsv(adult.DateOfBirth.ToString("M/d/yyyy", CultureInfo.InvariantCulture))
                            );
                        }
                        else if (person is Child child)
                        {
                            line = string.Join(",",
                                EscapeCsv(household.HouseNumber.ToString()),
                                EscapeCsv(household.Address),
                                EscapeCsv(child.Id),
                                "Child",
                                EscapeCsv(child.FullName),
                                child.Age,
                                EscapeCsv(child.School),
                                EscapeCsv(child.BirthCertificateNumber),
                                EscapeCsv(child.DateOfBirth.ToString("M/d/yyyy", CultureInfo.InvariantCulture)),
                                child.Grade
                            );
                        }
                        else continue;
                        sb.AppendLine(line);
                    }
                }
                File.WriteAllText(filename, sb.ToString());
            }
            catch (Exception ex)
            {
                throw new IOException($"Error writing to file: {ex.Message}", ex);
            }
        }
    }
}
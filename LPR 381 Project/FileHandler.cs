using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace LPR_381_Project
{
    public static class FileHandler
    {
        public static LinearProgramModel LoadLinearProgramFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.");

            var model = new LinearProgramModel();
            var lines = File.ReadAllLines(filePath);

            if (lines.Length < 3) // Ensure there are enough lines
            {
                throw new FormatException("File format is invalid. It must contain at least 3 lines.");
            }

            // Read the first line
            var firstLine = lines[0].Split(' ');
            if (firstLine.Length < 2) // Check for correct number of elements
            {
                throw new FormatException("The first line must contain the maximization/minimization keyword and coefficients.");
            }

            model.IsMaximization = firstLine[0].Trim().ToLower() == "max";
            model.ObjectiveCoefficients = new List<double>(Array.ConvertAll(firstLine.Skip(1).ToArray(), double.Parse));

            // Read constraints
            model.Constraints = new List<Constraint>();
            for (int i = 1; i < lines.Length - 1; i++)
            {
                var parts = lines[i].Split(' ');
                if (parts.Length < 3) // Check for correct number of elements
                {
                    throw new FormatException($"Constraint format is invalid on line {i + 1}.");
                }
                List<double> constrs = new List<double>();
                   for (int j = 0; j < parts.Length-2; j++)
                {
                    constrs.Add(double.Parse(parts[j]));
                }
                 string relation = parts[parts.Length-2];
                double Rhs = double.Parse(parts[parts.Length-1]);



                Constraint constraint = new Constraint(constrs, relation, Rhs);
            
                model.Constraints.Add(constraint);
            }

            // Read sign restrictions
            var signRestrictions = lines[lines.Length - 1].Split(' ');
            model.SignRestrictions = new List<string>(signRestrictions);

            return model;
        }

        public static void SaveResultsToFile(string filePath, string results)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.");

            if (results == null)
                throw new ArgumentNullException(nameof(results), "Results cannot be null.");

            File.WriteAllText(filePath, results);
        }
    }
}

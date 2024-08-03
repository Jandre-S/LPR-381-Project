using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class PrimalSimplex
    {
        #region Example Of Use
        /*
              // Initialize LP
            PrimalSimplex lp = new PrimalSimplex();
            // Read LP from text file
            lp = lp.readProgramFromFile("C:\\Users\\Llewelyn\\OneDrive\\Desktop\\LP.txt");

            // Print the program as read from the file
            Console.WriteLine("Program as read from file:");
            lp.PrintProgram();

            // Convert to canonical form and print
            lp = lp.GetPrimalSimplexCanonical(lp);
            Console.WriteLine("Canonical form:");
            lp.PrintProgram();

            // Check if the program is optimal
            int isOptimal = lp.GetPrimalSimplexOptimalState(lp);
            if (isOptimal == 2)
            {
                do
                {
                    // Get pivot row and column
                    int[] pivots = lp.GetPivotRowAndColumnPrimalSimplex(lp);
                    // Perform pivot operation
                    lp = lp.Pivot(pivots[0], pivots[1], lp);
                    // Print the program after each pivot
                    Console.WriteLine("Program after pivot:");
                    lp.PrintProgram();
                    // Check if the program is optimal
                    isOptimal = lp.GetPrimalSimplexOptimalState(lp);
                } while (isOptimal == 2);
            }

            // Print the optimal program
            Console.WriteLine("Optimal program:");
            lp.PrintProgram();

            Console.ReadLine();
          */
        #endregion

        #region Variable Declarations
        public decimal[][] program;
        public string[] restrictions;
        public bool isMaximize;
        public string[] StSigns;
        public int variableCount;
        public bool isURS;
        #endregion

        #region Constructors

        public PrimalSimplex()
        {

        }

        public PrimalSimplex(decimal[][] programTable)
        {
            this.program = programTable;
        }

        public PrimalSimplex(decimal[][] program, string[] restrictions, string[] stSigns, bool maximize, int VariableCount, bool URS)
        {
            this.variableCount = VariableCount;
            this.restrictions = restrictions;
            this.program = program;
            this.isMaximize = maximize;
            this.StSigns = stSigns;
            this.isURS = URS;
        }


        #endregion

        #region Read Text File To Table
        public PrimalSimplex readProgramFromRTB(string text)
        {
            //create the initial program exactly like in Text file
            bool maximize = true;
            int decVarsCount = 0;
            // Split the text into lines
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int lineCount = lines.Length;
            string[] signs = new string[lineCount - 2];
            //just initialise. code monkey
            string[] restrictionRow = new string[1];
            // Initialize the decimal array with the line count
            decimal[][] newProgram = new decimal[lineCount - 1][];

            // Read the text line by line and populate the array
            int rowIndex = 0;
            foreach (string line in lines)
            {
                // Use Regex to split each line by any whitespace sequence
                string[] substrings = Regex.Split(line, @"\s+");
                //read the min max in first row
                if (rowIndex == 0)
                {
                    string minMax = substrings[0];
                    switch (minMax.ToLower())
                    {
                        case "max":
                            maximize = true;
                            break;
                        case "min":
                            maximize = false;
                            break;
                    }
                    //remove the min or max from array
                    substrings = substrings.Skip(1).ToArray();
                    //count the amount of variables at this point to exclude rhs
                    decVarsCount = substrings.Length;
                    //add rhs to the array as 0
                    string newElement = "0";
                    // Create a new array with one extra slot
                    string[] newSubstrings = new string[substrings.Length + 1];
                    // Copy existing elements
                    for (int i = 0; i < substrings.Length; i++)
                    {
                        newSubstrings[i] = substrings[i];
                    }
                    // Add the new element
                    newSubstrings[substrings.Length] = newElement;
                    // Replace the old array with the new array
                    substrings = newSubstrings;
                }
                //last row reserved for var restrictions
                if (rowIndex == lineCount - 1)
                {
                    restrictionRow = new string[substrings.Length];
                    restrictionRow = substrings;
                }
                else
                {
                    // Initialize the column count for this row
                    newProgram[rowIndex] = new decimal[substrings.Length];
                    //read, save and remove signs from the array if not first row and not last row
                    if (rowIndex != 0 && rowIndex != lineCount- 1)
                    {
                        string lastCol = substrings[substrings.Length - 1];
                        Regex regex = new Regex(@"(<=|>=|=)(\d+)");
                        Match match = regex.Match(lastCol);
                        if (match.Success)
                        {
                            signs[rowIndex - 1] = match.Groups[1].Value;
                            substrings[substrings.Length - 1] = match.Groups[2].Value;
                        }
                        else
                        {
                            throw new FormatException("Input did not match the expected format. Expected format: <=, >=, or = for sign restrictions");
                        }
                    }
                    // Convert substrings to decimals and populate the array for constraint rows
                    for (int i = 0; i < substrings.Length; i++)
                    {
                        if (decimal.TryParse(substrings[i], out decimal value))
                        {
                            newProgram[rowIndex][i] = value;
                        }
                        else
                        {
                            // Handle parsing errors if needed
                            Console.WriteLine($"Failed to convert '{substrings[i]}' to decimal.");
                        }
                    }
                }
                rowIndex++;
            }
            // Test if URS or not
            bool URS = true;
            for (int i = 0; i < newProgram[0].Length; i++)
            {
                if (newProgram[newProgram.Length - 1][i].ToString().ToLower() != "urs")
                {
                    URS = false;
                    break;
                }
            }
            //Transfer to Program with sign restrictions
            PrimalSimplex init = new PrimalSimplex(newProgram, restrictionRow, signs, maximize, decVarsCount, URS);
            return init;
        }

        #endregion

        #region Basic Pivot
        public PrimalSimplex Pivot(int pivotRow, int pivotColumn, PrimalSimplex lp)
        {
            // Get the value at the pivot position
            decimal pivotValue = lp.program[pivotRow][pivotColumn];

            // Create a new program to hold the pivoted result
            decimal[][] newProgram = new decimal[lp.program.Length][];
            for (int i = 0; i < lp.program.Length; i++)
            {
                newProgram[i] = new decimal[lp.program[i].Length];
            }

            /*
             Array.Length gives you the number of rows
             Array[i].Length gives you the number of columns in the i-th row.
              */

            //Update the pivot row first as later operations are dependent on divided values
            for (int i = 0; i < newProgram[pivotRow].Length; i++)
            {
                newProgram[pivotRow][i] = lp.program[pivotRow][i] / pivotValue;
            }

            for (int i = 0; i < newProgram.Length; i++)
            {
                for (int j = 0; j < newProgram[i].Length; j++)
                {
                    if (!(i == pivotRow))
                    {
                        newProgram[i][j] = (lp.program[i][j]) - ((lp.program[i][pivotColumn]) * (newProgram[pivotRow][j]));
                    }
                }
            }

            // Update the program with the new pivoted values
            lp.program = newProgram;
            return lp;
        }

        #endregion

        #region Get Primal Simplex Canonical

        public PrimalSimplex GetPrimalSimplexCanonical(PrimalSimplex lp)
        {
            decimal[] tempRHS = new decimal[lp.program.Length];
            decimal[][] tempNonRHS = new decimal[lp.program.Length][];
            PrimalSimplex canonicalProgram = new PrimalSimplex();

            // Initialize the inner arrays for tempNonRHS
            for (int i = 0; i < tempNonRHS.Length; i++)
            {
                tempNonRHS[i] = new decimal[lp.program[0].Length - 1];
            }

            // Negate the objective function coefficients
            for (int i = 0; i < lp.program[0].Length - 1; i++)
            {
                lp.program[0][i] = lp.program[0][i] * -1;
            }

            // Calculate the number of slack variables
            int slackAmount = lp.program.Length - 1; // Exclude z row

            // Initialize the slacks and canonical arrays
            decimal[][] slacks = new decimal[slackAmount][];
            decimal[][] canonical = new decimal[lp.program.Length][];

            for (int i = 0; i < slackAmount; i++)
            {
                slacks[i] = new decimal[slackAmount];
            }

            for (int i = 0; i < lp.program.Length; i++)
            {
                canonical[i] = new decimal[slackAmount + lp.program[0].Length];
            }

            // Generate column values for each of the slacks
            for (int i = 0; i < slackAmount; i++) // By row
            {
                for (int j = 0; j < slackAmount; j++) // By column
                {
                    if (j == i)
                    {
                        slacks[i][j] = 1;
                    }
                    else
                    {
                        slacks[i][j] = 0;
                    }
                }
            }

            // Store non-RHS separately
            for (int i = 0; i < lp.program.Length; i++)
            {
                for (int j = 0; j < lp.program[0].Length - 1; j++)
                {
                    tempNonRHS[i][j] = lp.program[i][j];
                }
            }

            // Store RHS separately
            for (int i = 0; i < lp.program.Length; i++)
            {
                tempRHS[i] = lp.program[i][lp.program[0].Length - 1];
            }

            // Combine all stored values to canonical table and return
            // Old table decision variables
            for (int i = 0; i < lp.program.Length; i++) // By row old table
            {
                for (int j = 0; j < lp.program[0].Length - 1; j++) // By column old table
                {
                    canonical[i][j] = tempNonRHS[i][j];
                }
            }

            // New table slacks
            for (int i = 0; i < slackAmount; i++) // By row
            {
                for (int j = 0; j < slackAmount; j++) // By column
                {
                    canonical[i + 1][j + lp.program[0].Length - 1] = slacks[i][j];
                }
            }

            // Return RHS
            for (int i = 0; i < tempRHS.Length; i++)
            {
                canonical[i][canonical[0].Length - 1] = tempRHS[i];
            }

            lp.program = canonical;
            return lp;
        }


        #endregion

        #region Get Primal Simplex Optimal State
        //return 1 optimal , 2 not optimal , 3 Optimal needs advanced operations
        public int GetPrimalSimplexOptimalStateBasic(PrimalSimplex lp)
        {
            for (int i = 0; i < lp.variableCount; i++)
            {
                if (program[0][i] < 0)
                {
                    return 2;
                }

            }

            return 1;
        }
        #endregion

        #region Get Primal Simplex State Non URS
        public int GetPrimalSimplexStateNonURSINT(PrimalSimplex lp)
        {

            //check ints
            // check bin 
            // set truncate int sensitivity
            int decimalPlacesSensitivity = 5;

            //test for basic variables
            int[] Basics =  new int[lp.variableCount];
            for (int i = 0; i < variableCount; i++)
            {
                bool foundOne = false;
                bool isBasic = true; // Assume column is basic initially
                for (int j = 0; j < lp.program.Length; j++)
                {
                    if (lp.program[j][i] == 1)
                    {
                        if (foundOne)
                        {
                            isBasic = false; // More than one '1' found
                        }
                        else
                        {
                            foundOne = true;
                        }
                    }
                    else if (lp.program[j][i] != 0)
                    {
                        isBasic = false; // Non-zero and non-one value found
                    }
                }

                // After checking all elements in the column
                if (!foundOne)
                {
                    isBasic = false; // No '1' found
                }

                // Save the result to the Basics array
                Basics[i] = isBasic ? 1 : 0;
            }
            bool containsInteger = false;
            //Test Basics against INT
            foreach (string rest in restrictions)
            {
                if(rest.ToLower() == "int")
                {
                    containsInteger = true;
                    break;
                }
            }
            // if integer constraint exists, test if basics are integer
            if (containsInteger)
            {
                int colPos = -1; // will start at 0 , basics already var count
                foreach (int var in Basics)
                {
                    colPos++;
                    //if basic, test if integer. if not integer return non optimal
                    if (var ==1)
                    {
                        //find basic row position again 
                        for (int i = 0; i < program.Length; i++)
                        {
                            if (program[i][colPos] == 1)
                            {
                                bool willRun = IsIntegerAfterTruncate((program[i][program[0].Length - 1]), decimalPlacesSensitivity);
                                if (!IsIntegerAfterTruncate( (program[i][program[0].Length-1]), decimalPlacesSensitivity))
                                {
                                    return 0;
                                }
                            }
                        }
                    }
                }
            }
     
            return 1;
        }
        #endregion

        #region Get Pivot Row and Column

        public int[] GetPivotRowAndColumnPrimalSimplex(PrimalSimplex lp)
        {
            int[] pivotRowAndColumn = new int[2];
            int biggestNegativePosition = -1;
            decimal biggestNegativeValue = decimal.MaxValue;

            // Check for pivot column first
            for (int i = 0; i < lp.program[0].Length - 1; i++)
            {
                if (lp.program[0][i] < 0 && lp.program[0][i] < biggestNegativeValue)
                {
                    biggestNegativeValue = lp.program[0][i];
                    biggestNegativePosition = i;
                }
            }

            // If no valid pivot column is found, return an invalid state
            if (biggestNegativePosition == -1)
            {
                return new int[] { -1, -1 }; // No pivot column found
            }

            // Do ratio check (negative 0 values should not be considered)
            decimal[] ratios = new decimal[lp.program.Length - 1];
            for (int i = 1; i < lp.program.Length; i++)
            {
                if (lp.program[i][biggestNegativePosition] > 0) // Only consider positive values
                {
                    ratios[i - 1] = lp.program[i][lp.program[0].Length - 1] / lp.program[i][biggestNegativePosition];
                }
                else
                {
                    ratios[i - 1] = decimal.MaxValue; // Ignore negative or zero values
                }
            }

            // Find the position of the smallest ratio
            int smallestRatioPosition = -1;
            decimal smallestRatio = decimal.MaxValue;
            for (int i = 0; i < ratios.Length; i++)
            {
                if (ratios[i] >= 0 && ratios[i] < smallestRatio)
                {
                    smallestRatio = ratios[i];
                    smallestRatioPosition = i;
                }
            }

            // If no valid pivot row is found, return an invalid state
            if (smallestRatioPosition == -1)
            {
                return new int[] { -1, -1 }; // No pivot row found
            }

            // Set pivot row and column
            pivotRowAndColumn[0] = smallestRatioPosition + 1; // Adjusting for the row index (as rows start from 1)
            pivotRowAndColumn[1] = biggestNegativePosition;

            return pivotRowAndColumn;
        }


        #endregion

        #region Print LP to RTB
        public string PrintProgram()
        {
            string programToText = "";
            for (int i = 0; i < program.Length; i++)
            {
                for (int j = 0; j < program[i].Length; j++)
                {
                   programToText += ($"{program[i][j]:0.00}\t");
                }
                programToText += "\n";
            }
            programToText += "\n";
            return programToText;
        }
        #endregion

        #region Private Methods

        private bool IsIntegerAfterTruncate(decimal number, int decimalPlaces)
        {
            decimal factor = (decimal)Math.Pow(10, decimalPlaces);
            decimal truncatedValue = Math.Truncate(number * factor) / factor;
            bool returnValue = truncatedValue == number;
            return truncatedValue == number;
        }

        #endregion

    }
}

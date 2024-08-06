using System;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Represents the Branch and Bound Simplex algorithm for solving integer linear programming problems.
    /// </summary>
    internal class BnBSimplex
    {
        private PrimalSimplex primalSimplex;
        private decimal bestObjectiveValue;
        private decimal[][] bestSolution;
        private List<PrimalSimplex> subproblems;

        /// <summary>
        /// Initializes a new instance of the <see cref="BnBSimplex"/> class.
        /// </summary>
        public BnBSimplex()
        {
            primalSimplex = new PrimalSimplex();
            bestObjectiveValue = decimal.MinValue;
            subproblems = new List<PrimalSimplex>();
        }

        /// <summary>
        /// Creates a hard-coded test case for the Branch and Bound Simplex algorithm.
        /// </summary>
        /// <returns>A <see cref="PrimalSimplex"/> object representing the test case.</returns>
        private PrimalSimplex CreateTestCase()
        {
            decimal[][] program = new decimal[][]
            {
                new decimal[] { 2, 3, 3, 5, 2, 4, 0 },  // Objective function
                new decimal[] { 11, 8, 6, 14, 10, 10, 40 }  // Constraint
            };

            string[] restrictions = new string[] { "BIN", "BIN", "BIN", "BIN", "BIN", "BIN" };
            string[] stSigns = new string[] { "<=" };
            bool isMaximize = true;
            int variableCount = 6;
            bool isURS = false;

            return new PrimalSimplex(program, restrictions, stSigns, isMaximize, variableCount, isURS);
        }

        /// <summary>
        /// Solves the integer linear programming problem using the Branch and Bound Simplex algorithm.
        /// </summary>
        /// <returns>A 2D decimal array representing the best solution found.</returns>
        public decimal[][] Solve()
        {
            PrimalSimplex initialProblem = CreateTestCase();
            subproblems.Add(initialProblem);

            Console.WriteLine("Initial Problem:");
            PrintProblem(initialProblem);

            while (subproblems.Count > 0)
            {
                PrimalSimplex currentProblem = subproblems[0];
                subproblems.RemoveAt(0);

                SolveSubproblem(currentProblem);
            }

            Console.WriteLine("\nFinal Solution:");
            if (bestSolution != null)
            {
                PrintSolution(bestSolution);
            }
            else
            {
                Console.WriteLine("No feasible integer solution found.");
            }

            return bestSolution;
        }

        /// <summary>
        /// Prints the details of the given problem to the console.
        /// </summary>
        /// <param name="problem">The <see cref="PrimalSimplex"/> object representing the problem to be printed.</param>
        private void PrintProblem(PrimalSimplex problem)
        {
            Console.Write($"{(problem.isMaximize ? "Maximize" : "Minimize")}: ");
            for (int i = 0; i < problem.variableCount; i++)
            {
                Console.Write($"{(problem.program[0][i] >= 0 ? "+" : "")}{problem.program[0][i]}x{i + 1} ");
            }
            Console.WriteLine();

            for (int i = 1; i < problem.program.Length; i++)
            {
                for (int j = 0; j < problem.variableCount; j++)
                {
                    Console.Write($"{(problem.program[i][j] >= 0 ? "+" : "")}{problem.program[i][j]}x{j + 1} ");
                }
                Console.WriteLine($"{problem.StSigns[i - 1]} {problem.program[i][problem.program[i].Length - 1]}");
            }

            Console.WriteLine("All variables are binary.");
        }

        /// <summary>
        /// Prints the solution details to the console.
        /// </summary>
        /// <param name="solution">A 2D decimal array representing the solution to be printed.</param>
        private void PrintSolution(decimal[][] solution)
        {
            Console.WriteLine($"Objective Value: {solution[0][solution[0].Length - 1]}");
            for (int i = 0; i < solution[0].Length - 1; i++)
            {
                Console.WriteLine($"x{i + 1} = {solution[1][i]}");
            }
        }

        /// <summary>
        /// Solves a subproblem of the Branch and Bound Simplex algorithm.
        /// </summary>
        /// <param name="problem">The <see cref="PrimalSimplex"/> object representing the subproblem to be solved.</param>
        private void SolveSubproblem(PrimalSimplex problem)
        {
            problem = problem.GetPrimalSimplexCanonical(problem);

            int isOptimal;
            do
            {
                int[] pivots = problem.GetPivotRowAndColumnPrimalSimplex(problem);
                if (pivots[0] == -1 || pivots[1] == -1)
                {
                    // Problem is infeasible or unbounded
                    return;
                }
                problem = problem.Pivot(pivots[0], pivots[1], problem);
                isOptimal = problem.GetPrimalSimplexOptimalStateBasic(problem);
            } while (isOptimal == 2);

            if (IsIntegerSolution(problem))
            {
                UpdateBestSolution(problem);
            }
            else if (problem.program[0][problem.program[0].Length - 1] > bestObjectiveValue)
            {
                // Branch
                int branchVariable = GetBranchVariable(problem);
                decimal value = problem.program[1][branchVariable];
                decimal floorValue = Math.Floor(value);
                decimal ceilValue = Math.Ceiling(value);

                PrimalSimplex lowerBound = CreateBoundedProblem(problem, branchVariable, "<=", floorValue);
                PrimalSimplex upperBound = CreateBoundedProblem(problem, branchVariable, ">=", ceilValue);

                subproblems.Add(lowerBound);
                subproblems.Add(upperBound);
            }
        }

        /// <summary>
        /// Determines if the given problem has an integer solution.
        /// </summary>
        /// <param name="problem">The <see cref="PrimalSimplex"/> object representing the problem to be checked.</param>
        /// <returns><c>true</c> if the solution is an integer solution; otherwise, <c>false</c>.</returns>
        private bool IsIntegerSolution(PrimalSimplex problem)
        {
            for (int i = 1; i < problem.program.Length; i++)
            {
                if (!IsInteger(problem.program[i][problem.program[i].Length - 1]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if the given value is an integer.
        /// </summary>
        /// <param name="value">The decimal value to be checked.</param>
        /// <returns><c>true</c> if the value is an integer; otherwise, <c>false</c>.</returns>
        private bool IsInteger(decimal value)
        {
            return Math.Abs(value - Math.Round(value)) < 0.000001m;
        }

        /// <summary>
        /// Updates the best solution found so far with the given problem's solution.
        /// </summary>
        /// <param name="problem">The <see cref="PrimalSimplex"/> object representing the problem with the new solution.</param>
        private void UpdateBestSolution(PrimalSimplex problem)
        {
            decimal objectiveValue = problem.program[0][problem.program[0].Length - 1];
            if (objectiveValue > bestObjectiveValue)
            {
                bestObjectiveValue = objectiveValue;
                bestSolution = problem.program;
            }
        }

        /// <summary>
        /// Gets the variable to branch on from the given problem.
        /// </summary>
        /// <param name="problem">The <see cref="PrimalSimplex"/> object representing the problem.</param>
        /// <returns>The index of the variable to branch on.</returns>
        private int GetBranchVariable(PrimalSimplex problem)
        {
            for (int i = 0; i < problem.variableCount; i++)
            {
                if (!IsInteger(problem.program[1][i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Creates a new subproblem with an additional constraint based on the original problem.
        /// </summary>
        /// <param name="original">The original <see cref="PrimalSimplex"/> problem.</param>
        /// <param name="variable">The index of the variable to add the constraint on.</param>
        /// <param name="sign">The sign of the new constraint ("<=" or ">=").</param>
        /// <param name="bound">The bound value for the new constraint.</param>
        /// <returns>A new <see cref="PrimalSimplex"/> object representing the subproblem with the additional constraint.</returns>
        private PrimalSimplex CreateBoundedProblem(PrimalSimplex original, int variable, string sign, decimal bound)
        {
            decimal[][] newProgram = new decimal[original.program.Length + 1][];
            for (int i = 0; i < original.program.Length; i++)
            {
                newProgram[i] = (decimal[])original.program[i].Clone();
            }
            newProgram[original.program.Length] = new decimal[original.program[0].Length];
            newProgram[original.program.Length][variable] = sign == "<=" ? 1 : -1;
            newProgram[original.program.Length][newProgram[0].Length - 1] = bound;

            string[] newRestrictions = new string[original.restrictions.Length + 1];
            Array.Copy(original.restrictions, newRestrictions, original.restrictions.Length);
            newRestrictions[newRestrictions.Length - 1] = "INT";

            string[] newStSigns = new string[original.StSigns.Length + 1];
            Array.Copy(original.StSigns, newStSigns, original.StSigns.Length);
            newStSigns[newStSigns.Length - 1] = sign;

            return new PrimalSimplex(newProgram, newRestrictions, newStSigns, original.isMaximize, original.variableCount, original.isURS);
        }
    }
}

using System;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    internal class BranchAndBoundSimplex
    {
        private PrimalSimplex primalSimplex;
        private decimal bestObjectiveValue;
        private decimal[][] bestSolution;
        private List<PrimalSimplex> subproblems;

        public BranchAndBoundSimplex()
        {
            primalSimplex = new PrimalSimplex();
            bestObjectiveValue = decimal.MinValue;
            subproblems = new List<PrimalSimplex>();
        }

        public decimal[][] Solve(PrimalSimplex initialProblem)
        {
            subproblems.Add(initialProblem);

            while (subproblems.Count > 0)
            {
                PrimalSimplex currentProblem = subproblems[0];
                subproblems.RemoveAt(0);

                SolveSubproblem(currentProblem);
            }

            return bestSolution;
        }

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

        private bool IsInteger(decimal value)
        {
            return Math.Abs(value - Math.Round(value)) < 0.000001m;
        }

        private void UpdateBestSolution(PrimalSimplex problem)
        {
            decimal objectiveValue = problem.program[0][problem.program[0].Length - 1];
            if (objectiveValue > bestObjectiveValue)
            {
                bestObjectiveValue = objectiveValue;
                bestSolution = problem.program;
            }
        }

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

        private PrimalSimplex CreateBoundedProblem(PrimalSimplex original, int variable, string sign, decimal bound)
        {
            // Create a new constraint and add it to the problem
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

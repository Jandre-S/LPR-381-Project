using System;
using System.Collections.Generic;
using System.Linq;

namespace LPR_381_Project
{
    public class SimplexSolver
    {
        public LPRelaxationSolution Solve(LinearProgramModel model)
        {
            int numConstraints = model.Constraints.Count;
            int numVariables = model.ObjectiveCoefficients.Count;

            // Step 1: Initialize the simplex tableau
            double[,] tableau = InitializeTableau(model);

            while (true)
            {
                // Step 2: Check for optimality (no negative coefficients in the objective row)
                int pivotColumn = FindPivotColumn(tableau, numConstraints, numVariables);
                if (pivotColumn == -1)
                {
                    // Optimal solution found
                    return ExtractSolution(tableau, model);
                }

                // Step 3: Determine the pivot row
                int pivotRow = FindPivotRow(tableau, pivotColumn, numConstraints, numVariables);
                if (pivotRow == -1)
                {
                    // Unbounded solution
                    throw new InvalidOperationException("Linear program is unbounded.");
                }

                // Step 4: Pivot around the pivot element
                Pivot(tableau, pivotRow, pivotColumn, numConstraints, numVariables);
            }
        }

        private double[,] InitializeTableau(LinearProgramModel model)
        {
            int numConstraints = model.Constraints.Count;
            int numVariables = model.ObjectiveCoefficients.Count;

            // +1 for the RHS column
            double[,] tableau = new double[numConstraints + 1, numVariables + numConstraints + 1];

            // Initialize objective function row
            for (int j = 0; j < numVariables; j++)
            {
                tableau[numConstraints, j] = model.IsMaximization ? -model.ObjectiveCoefficients[j] : model.ObjectiveCoefficients[j];
            }

            // Initialize constraints rows
            for (int i = 0; i < numConstraints; i++)
            {
                var constraint = model.Constraints[i];
                for (int j = 0; j < numVariables; j++)
                {
                    tableau[i, j] = constraint.Coefficients[j];
                }
                tableau[i, numVariables + i] = 1.0; // Slack variable
                tableau[i, numVariables + numConstraints] = constraint.RightHandSide; // RHS value
            }

            return tableau;
        }

        private int FindPivotColumn(double[,] tableau, int numConstraints, int numVariables)
        {
            int lastRow = numConstraints;
            int pivotColumn = -1;
            double minValue = 0;

            for (int j = 0; j < numVariables + numConstraints; j++)
            {
                // Ensure we are not out of bounds when accessing the tableau
                if (j < tableau.GetLength(1))
                {
                    if (tableau[lastRow, j] < minValue)
                    {
                        minValue = tableau[lastRow, j];
                        pivotColumn = j;
                    }
                }
                else
                {
                    // Log this error to help debug any out-of-bounds access
                    Console.WriteLine($"Warning: Attempted to access out-of-bounds index {j} in FindPivotColumn.");
                    throw new ArgumentOutOfRangeException("Index out of range in tableau access.");
                }
            }

            return pivotColumn;
        }

        private int FindPivotRow(double[,] tableau, int pivotColumn, int numConstraints, int numVariables)
        {
            int pivotRow = -1;
            double minRatio = double.PositiveInfinity;

            for (int i = 0; i < numConstraints; i++)
            {
                double rhs = tableau[i, numVariables + numConstraints];
                double pivotColumnValue = tableau[i, pivotColumn];

                if (pivotColumnValue > 0)
                {
                    double ratio = rhs / pivotColumnValue;
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                }
            }

            return pivotRow;
        }

        private void Pivot(double[,] tableau, int pivotRow, int pivotColumn, int numConstraints, int numVariables)
        {
            int numRows = numConstraints + 1;
            int numColumns = numVariables + numConstraints + 1;

            double pivotValue = tableau[pivotRow, pivotColumn];

            // Divide the pivot row by the pivot element
            for (int j = 0; j < numColumns; j++)
            {
                tableau[pivotRow, j] /= pivotValue;
            }

            // Update the rest of the tableau
            for (int i = 0; i < numRows; i++)
            {
                if (i != pivotRow)
                {
                    double multiplier = tableau[i, pivotColumn];
                    for (int j = 0; j < numColumns; j++)
                    {
                        tableau[i, j] -= multiplier * tableau[pivotRow, j];
                    }
                }
            }
        }

        private LPRelaxationSolution ExtractSolution(double[,] tableau, LinearProgramModel model)
        {
            int numConstraints = model.Constraints.Count;
            int numVariables = model.ObjectiveCoefficients.Count;
            double[] solution = new double[numVariables];

            for (int j = 0; j < numVariables; j++)
            {
                bool isBasic = true;
                int basicRow = -1;

                for (int i = 0; i < numConstraints; i++)
                {
                    if (Math.Abs(tableau[i, j] - 1.0) < 1e-6)
                    {
                        if (basicRow == -1)
                        {
                            basicRow = i;
                        }
                        else
                        {
                            isBasic = false;
                            break;
                        }
                    }
                    else if (Math.Abs(tableau[i, j]) > 1e-6)
                    {
                        isBasic = false;
                        break;
                    }
                }

                if (isBasic && basicRow != -1)
                {
                    solution[j] = tableau[basicRow, numVariables + numConstraints];
                }
                else
                {
                    solution[j] = 0.0;
                }
            }

            return new LPRelaxationSolution
            {
                IsFeasible = true,
                ObjectiveValue = tableau[numConstraints, numVariables + numConstraints],
                Variables = solution
            };
        }
    }
}

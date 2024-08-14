using System;
using System.Collections.Generic;
using System.Linq;

namespace LPR_381_Project
{
    public class SensitivityAnalysis
    {
        private LinearProgramModel _model;
        private LinearProgramSolver _solver;
        private double[] _currentObjectiveCoefficients;
        private double[] _currentRightHandSide;
        private double[] _shadowPrices;

        public SensitivityAnalysis(LinearProgramModel model, LinearProgramSolver solver)
        {
            _model = model;
            _solver = solver;
            _currentObjectiveCoefficients = model.GetObjectiveCoefficients().ToArray();
            _currentRightHandSide = model.Constraints.Select(c => c.RightHandSide).ToArray();
            _shadowPrices = solver.GetShadowPrices().ToArray();
        }

        public void AnalyzeObjectiveFunction(double[] newCoefficients)
        {
            if (newCoefficients.Length != _currentObjectiveCoefficients.Length)
                throw new ArgumentException("New coefficients array length must match the current coefficients length.");

            double[] changes = new double[newCoefficients.Length];
            for (int i = 0; i < newCoefficients.Length; i++)
            {
                changes[i] = newCoefficients[i] - _currentObjectiveCoefficients[i];
            }

            // Evaluate impact on the solution
            Console.WriteLine("Objective Function Sensitivity Analysis:");
            for (int i = 0; i < changes.Length; i++)
            {
                Console.WriteLine($"Change in Coefficient {i}: {changes[i]}");
                // Optionally re-solve the LP with new coefficients and compare results
            }
        }

        public void AnalyzeConstraintCoefficients(double[,] newCoefficients)
        {
            if (newCoefficients.GetLength(0) != _model.Constraints.Count || newCoefficients.GetLength(1) != _model.ObjectiveCoefficients.Count)
                throw new ArgumentException("New coefficients matrix dimensions must match the constraints and variables.");

            // For simplicity, assume a 2D array of new coefficients for all constraints
            double[,] changes = new double[newCoefficients.GetLength(0), newCoefficients.GetLength(1)];
            for (int i = 0; i < newCoefficients.GetLength(0); i++)
            {
                for (int j = 0; j < newCoefficients.GetLength(1); j++)
                {
                    changes[i, j] = newCoefficients[i, j] - _model.GetConstraintCoefficients(i)[j];
                }
            }

            // Evaluate impact on the solution
            Console.WriteLine("Constraint Coefficients Sensitivity Analysis:");
            for (int i = 0; i < changes.GetLength(0); i++)
            {
                for (int j = 0; j < changes.GetLength(1); j++)
                {
                    Console.WriteLine($"Change in Coefficient at ({i}, {j}): {changes[i, j]}");
                    // Optionally re-solve the LP with new coefficients and compare results
                }
            }
        }

        public void AnalyzeRightHandSide(double[] newRightHandSide)
        {
            if (newRightHandSide.Length != _currentRightHandSide.Length)
                throw new ArgumentException("New right-hand side array length must match the current right-hand side length.");

            double[] changes = new double[newRightHandSide.Length];
            for (int i = 0; i < newRightHandSide.Length; i++)
            {
                changes[i] = newRightHandSide[i] - _currentRightHandSide[i];
            }

            // Evaluate impact on the solution
            Console.WriteLine("Right-Hand Side Sensitivity Analysis:");
            for (int i = 0; i < changes.Length; i++)
            {
                Console.WriteLine($"Change in RHS of Constraint {i}: {changes[i]}");
                // Optionally re-solve the LP with new RHS values and compare results
            }
        }

        public void DisplayShadowPrices()
        {
            Console.WriteLine("Shadow Prices:");
            for (int i = 0; i < _shadowPrices.Length; i++)
            {
                Console.WriteLine($"Constraint {i + 1}: {_shadowPrices[i]}");
            }
        }
    }
}

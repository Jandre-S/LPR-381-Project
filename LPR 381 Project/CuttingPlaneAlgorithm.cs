using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LPR_381_Project
{
    public class CuttingPlaneAlgorithm
    {
        private LinearProgramModel model;
        private SimplexSolver simplexSolver;
        private double[] bestSolution;
        private double bestObjectiveValue;
        private RichTextBox outputBox;

        public CuttingPlaneAlgorithm(LinearProgramModel model, SimplexSolver simplexSolver, RichTextBox outputBox)
        {
            this.model = model;
            this.simplexSolver = simplexSolver;
            this.outputBox = outputBox;
            this.bestSolution = new double[model.ObjectiveCoefficients.Count];
            this.bestObjectiveValue = double.NegativeInfinity;
        }

        public void Solve()
        {
            AppendOutput("Canonical Form:");
            DisplayCanonicalForm();

            // Initial solution from LP relaxation
            var lpSolution = simplexSolver.Solve(model);
            DisplayIteration(lpSolution, "Initial Solution");

            while (true)
            {
                if (lpSolution == null || !lpSolution.IsFeasible)
                {
                    AppendOutput("No feasible solution found.");
                    return;
                }

                if (IsIntegerSolution(lpSolution))
                {
                    // Update the best solution if the current one is better
                    if (lpSolution.ObjectiveValue > bestObjectiveValue)
                    {
                        bestObjectiveValue = lpSolution.ObjectiveValue;
                        bestSolution = lpSolution.Variables;
                    }
                    break;
                }

                // Generate a cutting plane based on the fractional solution
                var cuttingPlane = GenerateCuttingPlane(lpSolution.Variables);
                if (cuttingPlane == null)
                {
                    // No more cutting planes can be generated; terminate
                    break;
                }

                // Add the cutting plane to the model
                AddCuttingPlaneToModel(cuttingPlane);
                AppendOutput("Added Cutting Plane:");
                DisplayCuttingPlane(cuttingPlane);

                // Solve the updated LP model
                lpSolution = simplexSolver.Solve(model);
                DisplayIteration(lpSolution, "After Adding Cutting Plane");
            }

            // Output the best solution found
            AppendOutput("Best Objective Value: " + bestObjectiveValue);
            AppendOutput("Best Solution: " + string.Join(", ", bestSolution));
        }

        public string GetResults()
        {
            if (bestSolution == null)
            {
                return "No feasible solution found.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("Best Objective Value: " + bestObjectiveValue);
            sb.AppendLine("Best Solution: " + string.Join(", ", bestSolution));
            return sb.ToString();
        }

        private void DisplayCanonicalForm()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Objective Function:");
            sb.AppendLine(string.Join(" + ", model.ObjectiveCoefficients.Select((coef, i) => $"{coef}x{i + 1}")));

            sb.AppendLine("Constraints:");
            foreach (var constraint in model.Constraints)
            {
                var constraintStr = string.Join(" + ", constraint.Coefficients.Select((coef, i) => $"{coef}x{i + 1}"));
                sb.AppendLine($"{constraintStr} {constraint.Relation} {constraint.RightHandSide}");
            }

            AppendOutput(sb.ToString());
        }

        private void DisplayIteration(LPRelaxationSolution solution, string iterationDescription)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{iterationDescription}:");
            sb.AppendLine("Objective Value: " + solution.ObjectiveValue);
            sb.AppendLine("Solution: " + string.Join(", ", solution.Variables));

            AppendOutput(sb.ToString());
        }

        private void DisplayCuttingPlane(CuttingPlane cuttingPlane)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Cutting Plane:");
            sb.AppendLine("Coefficients: " + string.Join(", ", cuttingPlane.Coefficients));
            sb.AppendLine("Constant: " + cuttingPlane.Constant);

            AppendOutput(sb.ToString());
        }

        private void AppendOutput(string text)
        {
            if (outputBox.InvokeRequired)
            {
                outputBox.Invoke(new Action(() => outputBox.AppendText(text + Environment.NewLine)));
            }
            else
            {
                outputBox.AppendText(text + Environment.NewLine);
            }
        }

        private bool IsIntegerSolution(LPRelaxationSolution solution)
        {
            // Check if the solution contains only integer values
            return solution.Variables.All(v => Math.Abs(v - Math.Round(v)) < 1e-6);
        }

        private CuttingPlane GenerateCuttingPlane(double[] solution)
        {
            for (int i = 0; i < solution.Length; i++)
            {
                if (Math.Abs(solution[i] - Math.Round(solution[i])) > 1e-6)
                {
                    // Generate a cutting plane for the fractional part
                    var coefficients = new double[solution.Length];
                    coefficients[i] = 1.0;

                    return new CuttingPlane
                    {
                        Coefficients = coefficients,
                        Constant = Math.Floor(solution[i])
                    };
                }
            }

            // No cutting plane can be generated if the solution is integer
            return null;
        }

        private void AddCuttingPlaneToModel(CuttingPlane cuttingPlane)
        {
            if (model == null)
            {
                throw new InvalidOperationException("The LinearProgramModel has not been initialized.");
            }

            var newConstraint = new Constraint
            {
                Coefficients = cuttingPlane.Coefficients.ToList(),
                Relation = "<=",
                RightHandSide = cuttingPlane.Constant
            };

            model.Constraints.Add(newConstraint);
        }

        public double GetBestObjectiveValue() => bestObjectiveValue;
        public double[] GetBestSolution() => bestSolution;
    }

    public class CuttingPlane
    {
        public double[] Coefficients { get; set; }
        public double Constant { get; set; }
    }

    public class LPRelaxationSolution
    {
        public bool IsFeasible { get; set; }
        public double ObjectiveValue { get; set; }
        public double[] Variables { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1;

namespace LPR_381_Project
{
    public class LinearProgramSolver
    {
        private LinearProgramModel _model;

        public LinearProgramSolver(LinearProgramModel model)
        {
            _model = model;
        }

        public List<double> GetObjectiveCoefficients()
        {
            return _model.GetObjectiveCoefficients();
        }

        public List<double> GetConstraintCoefficients(int constraintIndex)
        {
            return _model.GetConstraintCoefficients(constraintIndex);
        }

        public double GetRightHandSide(int constraintIndex)
        {
            return _model.GetRightHandSide(constraintIndex);
        }

        public List<double> GetShadowPrices()
        {
            return _model.GetShadowPrices();
        }

        public void Solve(string algorithmName)
        {
            switch (algorithmName)
            {
                case "Primal Simplex":
                    SolvePrimalSimplex();
                    break;
                case "Revised Primal Simplex":
                    SolveRevisedPrimalSimplex();
                    break;
                case "Branch and Bound Simplex":
                    SolveBranchAndBoundSimplex();
                    break;
                case "Branch and Bound Knapsack":
                    SolveBranchAndBoundKnapsack();
                    break;
                case "Cutting Plane":
                    SolveCuttingPlane();
                    break;
                default:
                    throw new InvalidOperationException("Unknown algorithm selected.");
            }
        }

        private void SolvePrimalSimplex()
        {
            PrimalSimplexAlgorithm primalSimplex = new PrimalSimplexAlgorithm();
         //   primalSimplex.Solve(_model);
            // Process and display results as needed
        }

        private void SolveRevisedPrimalSimplex()
        {
            var revisedPrimalSimplex = new RevisedPrimalSimplexAlgorithm();
            revisedPrimalSimplex.Solve(_model);
            // Process and display results as needed
        }

        private void SolveBranchAndBoundSimplex()
        {
            var branchAndBoundSimplex = new BranchAndBoundSimplexAlgorithm();
            branchAndBoundSimplex.Solve(_model);
            // Process and display results as needed
        }

        private void SolveBranchAndBoundKnapsack()
        {
            var knapsackModel = new KnapsackModel
            {
                Items = new List<KnapsackItem>
            {
                new KnapsackItem { Index = 0, Value = 60, Weight = 10 },
                new KnapsackItem { Index = 1, Value = 100, Weight = 20 },
                new KnapsackItem { Index = 2, Value = 120, Weight = 30 }
            },
                Capacity = 50
            };
            var branchAndBoundKnapsack = new BranchAndBoundKnapsackAlgorithm(knapsackModel);
            branchAndBoundKnapsack.Solve();
            // Process and display results as needed
        }

        private void SolveCuttingPlane()
        {
            var cuttingPlane = new CuttingPlaneAlgorithm(_model);
            cuttingPlane.Solve();
            // Process and display results as needed
        }

        // Sensitivity Analysis Methods
        public string DisplayNonBasicVariableRange(int variableIndex)
        {
            // Implement sensitivity analysis for non-basic variables
            return "Range of selected Non-Basic Variable";
        }

        public string ApplyChangeToNonBasicVariable(int variableIndex, double newValue)
        {
            // Implement sensitivity analysis for applying changes to non-basic variables
            return "Change applied to Non-Basic Variable";
        }

        public string DisplayBasicVariableRange(int variableIndex)
        {
            // Implement sensitivity analysis for basic variables
            return "Range of selected Basic Variable";
        }

        public string ApplyChangeToBasicVariable(int variableIndex, double newValue)
        {
            // Implement sensitivity analysis for applying changes to basic variables
            return "Change applied to Basic Variable";
        }

        public string DisplayConstraintRightHandSideRange(int constraintIndex)
        {
            // Implement sensitivity analysis for constraint right-hand sides
            return "Range of selected Constraint RHS";
        }

        public string ApplyChangeToConstraintRightHandSide(int constraintIndex, double newValue)
        {
            // Implement sensitivity analysis for applying changes to constraint RHS
            return "Change applied to Constraint RHS";
        }

        public string DisplayVariableInNonBasicColumnRange(int variableIndex)
        {
            // Implement sensitivity analysis for variables in non-basic columns
            return "Range of selected Variable in Non-Basic Column";
        }

        public string ApplyChangeToVariableInNonBasicColumn(int variableIndex, double newValue)
        {
            // Implement sensitivity analysis for applying changes to variables in non-basic columns
            return "Change applied to Variable in Non-Basic Column";
        }

        public string AddNewActivityToOptimalSolution(double[] newActivityCoefficients)
        {
            // Implement adding new activities
            return "New activity added to optimal solution";
        }

        public string AddNewConstraintToOptimalSolution(double[] newConstraintCoefficients, double newConstraintRHS)
        {
            // Implement adding new constraints
            return "New constraint added to optimal solution";
        }

        public string DisplayShadowPrices()
        {
            // Implement displaying shadow prices
            return "Shadow prices displayed";
        }

        public string ApplyDuality()
        {
            // Implement duality
            return "Dual programming model applied";
        }

        public string VerifyDuality()
        {
            // Implement verification of duality
            return "Duality verified";
        }
    }
}

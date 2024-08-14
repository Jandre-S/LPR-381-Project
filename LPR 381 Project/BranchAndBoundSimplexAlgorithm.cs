using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApp1
{
    // The BranchAndBoundSimplex class implements the Branch and Bound algorithm 
    // combined with the Simplex method to solve linear programming problems with integer constraints.
    public class BranchAndBoundSimplexAlgorithm
    {
        // The Node class represents a node in the Branch and Bound tree.
        // Each node contains a possible solution, objective value, additional constraints, and a feasibility flag.
        private class Node
        {
            public double[] Solution { get; set; } // The solution vector for this node
            public double ObjectiveValue { get; set; } // The objective function value for this node
            public List<Constraint> AdditionalConstraints { get; set; } // Additional constraints imposed at this node
            public bool IsFeasible { get; set; } // Indicates whether this node is feasible
        }

        // The Constraint class represents a linear constraint imposed on a variable.
        private class Constraint
        {
            public int VariableIndex { get; set; } // The index of the variable involved in this constraint
            public bool IsLowerBound { get; set; } // Indicates whether this is a lower bound constraint
            public double Value { get; set; } // The value of the constraint (right-hand side)
        }

        // Coefficients of the objective function
        private double[] objectiveCoefficients;
        // Coefficients of the constraints
        private double[,] constraintCoefficients;
        // Right-hand sides of the constraints
        private double[] constraintRightHandSides;
        // Types of constraints (e.g., "≤", "≥", "=")
        private string[] constraintTypes;
        // Types of variables (e.g., "int", "bin", "continuous")
        private string[] variableTypes;
        // Flag indicating whether the objective is to maximize or minimize
        private bool isMaximization;
        // StringBuilder to accumulate output messages
        private StringBuilder output;

        // Constructor to initialize the BranchAndBoundSimplex class with problem data
        public BranchAndBoundSimplexAlgorithm(double[] objectiveCoefficients, double[,] constraintCoefficients,
            double[] constraintRightHandSides, string[] constraintTypes, string[] variableTypes, bool isMaximization)
        {
            this.objectiveCoefficients = objectiveCoefficients;
            this.constraintCoefficients = constraintCoefficients;
            this.constraintRightHandSides = constraintRightHandSides;
            this.constraintTypes = constraintTypes;
            this.variableTypes = variableTypes;
            this.isMaximization = isMaximization;
            this.output = new StringBuilder();
        }

        // The Solve method runs the Branch and Bound algorithm and returns the best integer solution found, 
        // along with a detailed output of the process.
        public (double[] Solution, string ProcessOutput) Solve()
        {
            try
            {
                // Create the root node (initial problem without any additional constraints)
                var rootNode = new Node
                {
                    AdditionalConstraints = new List<Constraint>(),
                    IsFeasible = true
                };

                // Queue to manage the nodes to be processed
                var nodeQueue = new Queue<Node>();
                nodeQueue.Enqueue(rootNode);

                Node bestIntegerSolution = null; // Best integer solution found so far
                double bestObjectiveValue = isMaximization ? double.NegativeInfinity : double.PositiveInfinity;
                string processOutput = "Branch and Bound Process:\n"; // String to store the process output

                // Main loop of the Branch and Bound algorithm
                while (nodeQueue.Count > 0)
                {

                    // Dequeue the next node to process
                    var currentNode = nodeQueue.Dequeue();

                    processOutput += $"Processing node with {currentNode.AdditionalConstraints.Count} additional constraints...\n";

                    // Skip if the node is not feasible
                    if (!currentNode.IsFeasible)
                    {
                        processOutput += "Node is not feasible. Skipping...\n";
                        continue;
                    }

                    // Solve the linear relaxation of the current node
                    output.Clear(); // Clear previous output
                    SolveRelaxation(currentNode);
                    processOutput += output.ToString(); // Add relaxation output to process output


                    // Skip if the relaxed solution is not feasible
                    if (!currentNode.IsFeasible)
                    {
                        processOutput += "Relaxed solution is not feasible. Skipping...\n";
                        continue;
                    }

                    processOutput += $"Objective value: {currentNode.ObjectiveValue:F4}\n";

                    // Check if the solution is integer
                    if (IsIntegerSolution(currentNode.Solution))
                    {
                        processOutput += "Found an integer solution.\n";
                        if (IsBetterSolution(currentNode.ObjectiveValue, bestObjectiveValue))
                        {
                            processOutput += "This is the best solution so far.\n";
                            bestIntegerSolution = currentNode; // Update the best solution
                            bestObjectiveValue = currentNode.ObjectiveValue;
                        }
                    }
                    // If the solution is not integer, branch and create child nodes
                    else if (IsBetterSolution(currentNode.ObjectiveValue, bestObjectiveValue))
                    {
                        int branchingVariableIndex = ChooseBranchingVariable(currentNode.Solution);
                        double branchingValue = Math.Floor(currentNode.Solution[branchingVariableIndex]);

                        processOutput += $"Branching on variable x{branchingVariableIndex + 1} at value {branchingValue:F4}.\n";

                        // Create and enqueue the child nodes for lower and upper bounds
                        var lowerBoundNode = CreateChildNode(currentNode, branchingVariableIndex, true, branchingValue);
                        var upperBoundNode = CreateChildNode(currentNode, branchingVariableIndex, false, branchingValue + 1);

                        nodeQueue.Enqueue(lowerBoundNode);
                        nodeQueue.Enqueue(upperBoundNode);
                    }
                }

                // Append the final result to the process output
                if (bestIntegerSolution != null)
                {
                    processOutput += "Best integer solution found.\n";
                }
                else
                {
                    processOutput += "No feasible integer solution found.\n";
                }

                // Return the best solution found and the process output
                return (bestIntegerSolution?.Solution, processOutput);

            }
            catch (Exception e)
            {
                return (null, $"Error occurred during solving:{e.Message}");
            }

        }

        // The SolveRelaxation method solves the linear relaxation of the current node
        private void SolveRelaxation(Node node)
        {
            int m = constraintRightHandSides.Length + node.AdditionalConstraints.Count;
            int n = objectiveCoefficients.Length;

            double[,] tableau = CreateInitialTableau(node);

            output.AppendLine("Initial Tableau:");
            PrintTableau(tableau);

            int iteration = 0;
            while (true)
            {
                iteration++;
                output.AppendLine($"\nIteration {iteration}:");

                int pivotColumn = FindPivotColumn(tableau);
                if (pivotColumn == -1)
                {
                    output.AppendLine("Optimal solution found.");
                    break;
                }

                int pivotRow = FindPivotRow(tableau, pivotColumn);
                if (pivotRow == -1)
                {
                    output.AppendLine("Problem is unbounded.");
                    node.IsFeasible = false;
                    return;
                }

                output.AppendLine($"Pivot: Row {pivotRow + 1}, Column {pivotColumn + 1}");
                PerformPivot(tableau, pivotRow, pivotColumn);

                output.AppendLine("Tableau after pivot:");
                PrintTableau(tableau);
            }

            // Extract the solution and objective value from the final tableau
            node.Solution = new double[n];
            for (int j = 0; j < n; j++)
            {
                int basicVariableRow = FindBasicVariableRow(tableau, j);
                node.Solution[j] = basicVariableRow != -1 ? tableau[basicVariableRow, m + n] : 0;
            }
            node.ObjectiveValue = isMaximization ? tableau[m, m + n] : -tableau[m, m + n];
            node.IsFeasible = true;

            output.AppendLine("\nFinal Solution:");
            for (int i = 0; i < node.Solution.Length; i++)
            {
                output.AppendLine($"x{i + 1} = {node.Solution[i]:F4}");
            }
            output.AppendLine($"Objective Value: {node.ObjectiveValue:F4}");
        }

        // The CreateInitialTableau method sets up the initial tableau for the Simplex method
        private double[,] CreateInitialTableau(Node node)
        {
            int m = constraintRightHandSides.Length + node.AdditionalConstraints.Count; // Total number of constraints
            int n = objectiveCoefficients.Length; // Number of variables
            double[,] tableau = new double[m + 1, m + n + 1]; // Tableau with m+1 rows and m+n+1 columns

            // Set up constraint rows for the original constraints
            for (int i = 0; i < constraintRightHandSides.Length; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    tableau[i, j] = constraintCoefficients[i, j];
                }
                tableau[i, n + i] = 1; // Slack variable
                tableau[i, m + n] = constraintRightHandSides[i];
            }

            // Set up additional constraint rows added by the Branch and Bound process
            for (int i = 0; i < node.AdditionalConstraints.Count; i++)
            {
                var constraint = node.AdditionalConstraints[i];
                tableau[constraintRightHandSides.Length + i, constraint.VariableIndex] = constraint.IsLowerBound ? 1 : -1;
                tableau[constraintRightHandSides.Length + i, n + constraintRightHandSides.Length + i] = 1;
                tableau[constraintRightHandSides.Length + i, m + n] = constraint.Value;
            }

            // Set up the objective function row
            for (int j = 0; j < n; j++)
            {
                tableau[m, j] = isMaximization ? -objectiveCoefficients[j] : objectiveCoefficients[j];
            }

            return tableau;
        }

        // The IsIntegerSolution method checks if a given solution is an integer solution
        private bool IsIntegerSolution(double[] solution)
        {
            for (int i = 0; i < solution.Length; i++)
            {
                if (variableTypes[i] == "int" || variableTypes[i] == "bin")
                {
                    if (Math.Abs(solution[i] - Math.Round(solution[i])) > 1e-5)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // The IsBetterSolution method checks if a given objective value is better than the current best value
        private bool IsBetterSolution(double objectiveValue, double bestObjectiveValue)
        {
            return isMaximization ? objectiveValue > bestObjectiveValue : objectiveValue < bestObjectiveValue;
        }

        // The ChooseBranchingVariable method selects a variable to branch on
        private int ChooseBranchingVariable(double[] solution)
        {
            for (int i = 0; i < solution.Length; i++)
            {
                if (variableTypes[i] == "int" || variableTypes[i] == "bin")
                {
                    if (Math.Abs(solution[i] - Math.Round(solution[i])) > 1e-5)
                    {
                        return i;
                    }
                }
            }
            throw new InvalidOperationException("No branching variable found.");
        }

        // The CreateChildNode method creates a child node for the Branch and Bound tree
        private Node CreateChildNode(Node parentNode, int variableIndex, bool isLowerBound, double value)
        {
            var childNode = new Node
            {
                AdditionalConstraints = new List<Constraint>(parentNode.AdditionalConstraints)
            };
            childNode.AdditionalConstraints.Add(new Constraint
            {
                VariableIndex = variableIndex,
                IsLowerBound = isLowerBound,
                Value = value
            });
            return childNode;
        }

        // The FindPivotColumn method identifies the pivot column in the tableau
        private int FindPivotColumn(double[,] tableau)
        {
            int m = tableau.GetLength(0) - 1; // Number of rows
            int n = tableau.GetLength(1) - 1; // Number of columns

            int pivotColumn = -1;
            double mostNegative = 0;

            for (int j = 0; j < n; j++)
            {
                if (tableau[m, j] < mostNegative)
                {
                    mostNegative = tableau[m, j];
                    pivotColumn = j;
                }
            }

            return pivotColumn;
        }

        // The FindPivotRow method identifies the pivot row in the tableau
        private int FindPivotRow(double[,] tableau, int pivotColumn)
        {
            int m = tableau.GetLength(0) - 1; // Number of rows
            int pivotRow = -1;
            double minimumRatio = double.PositiveInfinity;

            for (int i = 0; i < m; i++)
            {
                if (tableau[i, pivotColumn] > 0)
                {
                    double ratio = tableau[i, tableau.GetLength(1) - 1] / tableau[i, pivotColumn];
                    if (ratio < minimumRatio)
                    {
                        minimumRatio = ratio;
                        pivotRow = i;
                    }
                }
            }

            return pivotRow;
        }

        // The PerformPivot method performs the pivot operation on the tableau
        private void PerformPivot(double[,] tableau, int pivotRow, int pivotColumn)
        {
            int m = tableau.GetLength(0); // Number of rows
            int n = tableau.GetLength(1); // Number of columns

            double pivotElement = tableau[pivotRow, pivotColumn];

            // Normalize the pivot row
            for (int j = 0; j < n; j++)
            {
                tableau[pivotRow, j] /= pivotElement;
            }

            // Eliminate other entries in the pivot column
            for (int i = 0; i < m; i++)
            {
                if (i != pivotRow)
                {
                    double factor = tableau[i, pivotColumn];
                    for (int j = 0; j < n; j++)
                    {
                        tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }
            }
        }

        // The FindBasicVariableRow method identifies the row corresponding to a basic variable in the tableau
        private int FindBasicVariableRow(double[,] tableau, int variableIndex)
        {
            int m = tableau.GetLength(0) - 1; // Number of rows
            int basicRow = -1;

            for (int i = 0; i < m; i++)
            {
                if (Math.Abs(tableau[i, variableIndex] - 1) < 1e-5)
                {
                    if (basicRow != -1)
                    {
                        return -1; // Multiple entries of 1, not a basic variable
                    }
                    basicRow = i;
                }
                else if (Math.Abs(tableau[i, variableIndex]) > 1e-5)
                {
                    return -1; // Non-zero entry, not a basic variable
                }
            }

            return basicRow;
        }

        // The PrintTableau method outputs the current state of the tableau for debugging purposes
        private void PrintTableau(double[,] tableau)
        {
            int m = tableau.GetLength(0);
            int n = tableau.GetLength(1);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    output.Append($"{tableau[i, j]:F2}\t");
                }
                output.AppendLine();
            }
        }
    }
}

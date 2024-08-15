using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace LPR_381_Project
{
    public partial class Form1 : Form
    {
        private LinearProgramModel _model;
        private PrimalSimplexAlgorithm _primalSimplexAlgorithm;
        private RevisedPrimalSimplexAlgorithm _revisedPrimalSimplex;
        private BranchAndBoundSimplexAlgorithm _branchAndBoundSimplex;
        private BranchAndBoundKnapsackAlgorithm _branchAndBoundKnapsack;
        private CuttingPlaneAlgorithm _cuttingPlane;

        public Form1()
        {
            InitializeComponent();
            InitializeAlgorithms();
        }

        private void InitializeAlgorithms()
        {
            _primalSimplexAlgorithm = new PrimalSimplexAlgorithm();
            _revisedPrimalSimplex = new RevisedPrimalSimplexAlgorithm();
            // _branchAndBoundSimplex = new BranchAndBoundSimplexAlgorithm();

            // Initialize Knapsack model
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
            _branchAndBoundKnapsack = new BranchAndBoundKnapsackAlgorithm(knapsackModel);

            // Initialize SimplexSolver instance
            //var simplexSolver = new SimplexSolver();

            // Initialize CuttingPlaneAlgorithm with model and solver
            //_cuttingPlane = new CuttingPlaneAlgorithm(_model, simplexSolver);
        }

        private void toolStripButton_LoadFile_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Select an Input File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _model = FileHandler.LoadLinearProgramFromFile(openFileDialog.FileName);
                    DisplayModel();

                    // Initialize CuttingPlaneAlgorithm with the loaded model
                    var simplexSolver = new SimplexSolver(); // You may need to initialize or configure it as needed
                    _cuttingPlane = new CuttingPlaneAlgorithm(_model, simplexSolver, richTextBox_Solved);
                }
                catch (Exception ex)
                {
                    // Handle the case when the file cannot be loaded
                    LoadTestCase();
                    DisplayModel();
                }
            }
        }

        private void toolStripButton_Solve_Click(object sender, EventArgs e)
        {
            if (_model == null)
            {
                MessageBox.Show("Please load a model first.");
                return;
            }

            try
            {
                var selectedAlgorithm = toolStripComboBox_SolveAlgorithms.SelectedItem?.ToString();

                if (selectedAlgorithm == null)
                {
                    MessageBox.Show("Please select an algorithm.");
                    return;
                }

                switch (selectedAlgorithm)
                {
                    case "Primal Simplex":
                        // Your existing Primal Simplex code
                        break;
                    case "Revised Primal Simplex":
                        _revisedPrimalSimplex.Solve(_model);
                        break;
                    case "Branch and Bound Simplex":
                        if (_model != null)
                        {
                            // Your existing Branch and Bound Simplex code
                        }
                        else
                        {
                            MessageBox.Show("Please load a model first.");
                        }
                        break;
                    case "Branch and Bound Knapsack":
                        _branchAndBoundKnapsack.Solve();
                        break;
                    case "CuttingPlane":
                        if (_model == null)
                        {
                            Console.WriteLine("Model is not initialized.");
                            return;
                        }
                        var simplexSolver = new SimplexSolver();
                        _cuttingPlane = new CuttingPlaneAlgorithm(_model, simplexSolver, richTextBox_Solved);
                        _cuttingPlane.Solve();
                        break;
                    default:
                        MessageBox.Show("Invalid algorithm selected.");
                        return;
                }

                // Display results and save to file
                string results = "Results and analysis output"; // Replace with actual results
                FileHandler.SaveResultsToFile("output.txt", results);
                MessageBox.Show("Results saved to output.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error solving model: {ex.Message}");
            }
        }

        private void DisplayModel()
        {
            if (_model == null)
            {
                richTextBox_LoadFormFile.Clear();
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(_model.IsMaximization ? "Maximize" : "Minimize");

            // Objective function
            sb.Append("Objective Function: ");
            sb.AppendLine(string.Join(" ", _model.ObjectiveCoefficients));

            // Constraints
            sb.AppendLine("Constraints");
            foreach (var constraint in _model.Constraints)
            {
                sb.Append("Coefficients: ");
                sb.AppendLine(string.Join(" ", constraint.Coefficients));
                sb.Append($"Relation: {constraint.Relation} ");
                sb.AppendLine($"Right-Hand Side: {constraint.RightHandSide}");
            }

            // Sign Restrictions
            sb.Append("Sign Restrictions: ");
            sb.AppendLine(string.Join(" ", _model.SignRestrictions));

            richTextBox_LoadFormFile.Text = sb.ToString();
        }

        private double[,] ConvertConstraintsToArray(List<Constraint> constraints)
        {
            int rows = constraints.Count;
            int cols = constraints[0].Coefficients.Count;
            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = constraints[i].Coefficients[j];
                }
            }

            return result;
        }

        
        /// ///////////////////////////////////////////////////////////////////////
        /// Test Cae Model if Text file cant load
        // ///////////////////////////////////////////////////////////////////////
        private void LoadTestCase()
        {
            _model = new LinearProgramModel
            {
                IsMaximization = true,
                ObjectiveCoefficients = new List<double> { 2, 3, 3, 5, 2, 4 },
                Constraints = new List<Constraint>
        {
            new Constraint
            {
                Coefficients = new List<double> { 11, 8, 6, 14, 10, 10 },
                Relation = "<=",
                RightHandSide = 40
            }
        },
                SignRestrictions = new List<string> { "bin", "bin", "bin", "bin", "bin", "bin" }
            };
            DisplayModel();
        }
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Working
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////
        private void toolStripButton_NonLinear_Click(object sender, EventArgs e)
        {
            //_nonLinear = new NonLinear();
            //richTextBox_Solved.Text = _nonLinear.SolveExamples();

        }
    }
}

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
        private NonLinear _nonLinear;

        public Form1()
        {
            InitializeComponent();
            InitializeAlgorithms();
        }

        private void InitializeAlgorithms()
        {
            _primalSimplexAlgorithm = new PrimalSimplexAlgorithm();
            _revisedPrimalSimplex = new RevisedPrimalSimplexAlgorithm();
            _branchAndBoundSimplex = new BranchAndBoundSimplexAlgorithm();

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

            _cuttingPlane = new CuttingPlaneAlgorithm(new LinearProgramModel()); // Initialize with a default model
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
                }
                catch (Exception ex)
                {
                    //added fallback for when File doesn't load
                    //Demo Purposes
                    //MessageBox.Show($"Error loading file: {ex.Message}");
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
                // 
                //
                //
                // THIS WORKS
                //
                //
                //
                switch (selectedAlgorithm)
                {
                    case "Primal Simplex":
                        PrimalSimplexAlgorithm algo = new PrimalSimplexAlgorithm();
                      algo =   algo.ConvertToPrimal(_model);
                        richTextBox_Solved.Text += algo.PrintProgram();
                       algo = algo.GetPrimalSimplexCanonical(algo);
                        richTextBox_Solved.Text += algo.PrintProgram();
                        do
                        {
                            int[] pivots = algo.GetPivotRowAndColumnPrimalSimplex(algo);
                            algo.Pivot(pivots[0], pivots[1],algo);
                            richTextBox_Solved.Text += algo.PrintProgram();
                        } while (algo.GetPrimalSimplexOptimalStateBasic(algo) == 0);
                  //
                  //
                  //
                  //THIS WORKS
                  //
                  //
                  //
                  //


                        break;
                    case "Revised Primal Simplex":
                        _revisedPrimalSimplex.Solve(_model);
                        break;
                   
                    ////////////////////////////////////////////////////////////////////////
                    // DONT TOUCH
                    ////////////////////////////////////////////////////////////////////////
                    
                   case "Branch and Bound Simplex":
                        if (_model != null)
                        {
                            double[] objectiveCoefficients = _model.ObjectiveCoefficients.ToArray();
                            double[,] constraintCoefficients = ConvertConstraintsToArray(_model.Constraints);
                            double[] constraintRightHandSides = _model.Constraints.Select(c => c.RightHandSide).ToArray();
                            string[] constraintTypes = _model.Constraints.Select(c => c.Relation).ToArray();
                            string[] variableTypes = _model.SignRestrictions.ToArray();

                            _branchAndBoundSimplex = new BranchAndBoundSimplexAlgorithm(
                                objectiveCoefficients,
                                constraintCoefficients,
                                constraintRightHandSides,
                                constraintTypes,
                                variableTypes,
                                _model.IsMaximization
                            );

                            var (solution, processOutput) = _branchAndBoundSimplex.Solve();

                            richTextBox_Solved.Text = processOutput;
                            if (solution != null)
                            {
                                richTextBox_Solved.AppendText("\nFinal Solution:\n");
                                for (int i = 0; i < solution.Length; i++)
                                {
                                    richTextBox_Solved.AppendText($"x{i + 1} = {solution[i]:F4}\n");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please load a model first.");
                        }
                        break;
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    
                    case "Branch and Bound Knapsack":
                        _branchAndBoundKnapsack.Solve();
                        break;
                    case "Cutting Plane":
                        _cuttingPlane.Solve();
                        break;
                    default:
                        MessageBox.Show("Invalid algorithm selected.");
                        return;
                }

                // Display results and save to file
                string results = "Results and analysis output"; 
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
            _nonLinear = new NonLinear();
            richTextBox_Solved.Text = _nonLinear.SolveExamples();

        }
    }
}

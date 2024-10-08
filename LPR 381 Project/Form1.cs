﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LPR_381_Project
{
    public partial class Form1 : Form
    {
        private LinearProgramModel _model;
        private PrimalSimplexAlgorithm _primalSimplex;
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
            _primalSimplex = new PrimalSimplexAlgorithm();
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
                    MessageBox.Show($"Error loading file: {ex.Message}");
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
                        _primalSimplex.Solve(_model);
                        break;
                    case "Revised Primal Simplex":
                        _revisedPrimalSimplex.Solve(_model);
                        break;
                    case "Branch and Bound Simplex":
                        _branchAndBoundSimplex.Solve(_model);
                        break;
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
            sb.AppendLine("Constraints:");
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
    }
}

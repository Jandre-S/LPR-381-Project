using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void DisplayModelData()
        {
            rtbInitialProgram.Clear();
            rtbInitialProgram.AppendText($"Objective: {objectiveType}\r\n");
            rtbInitialProgram.AppendText("Objective Coefficients: " + string.Join(", ", objectiveCoefficients) + "\r\n");
            rtbInitialProgram.AppendText("Constraints:\r\n");
            foreach (var (coeffs, rel, rhs) in constraints)
            {
                rtbInitialProgram.AppendText($"{string.Join(" ", coeffs)} {rel} {rhs}\r\n");
            }
            rtbInitialProgram.AppendText("Sign Restrictions: " + string.Join(", ", signRestrictions) + "\r\n");
        }

        private string objectiveType;
        private List<double> objectiveCoefficients = new List<double>();
        private List<(double[] coefficients, string relation, double rhs)> constraints = new List<(double[] coefficients, string relation, double rhs)>();
        private List<string> signRestrictions = new List<string>();

        private void LoadModelFormTextFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length < 3)
                {
                    MessageBox.Show("Invalid input file format");
                }

                string[] objectiveParts = lines[0].Split(' ');
                objectiveType = objectiveParts[0].Trim();

                if (!objectiveType.Equals("max", StringComparison.OrdinalIgnoreCase) && !objectiveType.Equals("min", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Invalid objective type");
                }

                objectiveCoefficients = objectiveParts.Skip(1).Select(c => double.Parse(c)).ToList();

                constraints.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] constraintParts = lines[i].Split(' ');
                    double[] coefficients = constraintParts.Take(constraintParts.Length - 2).Select(c => double.Parse(c)).ToArray();
                    string relation = constraintParts[constraintParts.Length - 2];
                    double rhs = double.Parse(constraintParts.Last());
                    constraints.Add((coefficients, relation, rhs));
                }

                signRestrictions = lines.Last().Split(' ').ToList();

                DisplayModelData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            switch (cmbAlgorithmSelect.SelectedIndex) 
            {
            case 0:
                    SolvePrimalSimplexBasic();
                    break;
            }
        }

        private void SolvePrimalSimplexBasic()
        {
            rtbOperations.Clear();
            PrimalSimplex simplex = new PrimalSimplex();
            Operations ops = new Operations();
            string rtb = rtbInitialProgram.Text;
            simplex = simplex.readProgramFromRTB(rtb);
            rtbOperations.Text += simplex.PrintProgram();
            simplex.GetPrimalSimplexCanonical(simplex);
            rtbOperations.Text += simplex.PrintProgram();
            do
            {
                int[] pivots = simplex.GetPivotRowAndColumnPrimalSimplex(simplex);
                simplex.program = ops.Pivot(pivots[0], pivots[1], simplex.program);
                rtbOperations.Text += simplex.PrintProgram();
            }while (simplex.GetPrimalSimplexOptimalStateBasic(simplex) != 1);
            //check if optimal with int and bin 
            int nonURSState = simplex.GetPrimalSimplexStateNonURSINT(simplex);
             //if nonURSINT is false do branch and bound
             //if binary , fucked

        }

        private void btnSelectTextFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    LoadModelFormTextFile(filePath);
                }
            }
        }

        private void rtbInitialProgram_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

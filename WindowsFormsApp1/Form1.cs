using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        }
    }
}

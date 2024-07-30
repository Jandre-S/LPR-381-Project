using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class Operations
    {
        //overload this to pivot your table or use decimal[][]
        #region Basic Pivot
        public decimal[][] Pivot(int pivotRow, int pivotColumn, decimal[][] program)
        {
            // Get the value at the pivot position
            decimal pivotValue =program[pivotRow][pivotColumn];

            // Create a new program to hold the pivoted result
            decimal[][] newProgram = new decimal[program.Length][];
            for (int i = 0; i <program.Length; i++)
            {
                newProgram[i] = new decimal[program[i].Length];
            }

            /*
             Array.Length gives you the number of rows
             Array[i].Length gives you the number of columns in the i-th row.
              */

            //Update the pivot row first as later operations are dependent on divided values
            for (int i = 0; i < newProgram[pivotRow].Length; i++)
            {
                newProgram[pivotRow][i] =program[pivotRow][i] / pivotValue;
            }

            for (int i = 0; i < newProgram.Length; i++)
            {
                for (int j = 0; j < newProgram[i].Length; j++)
                {
                    if (!(i == pivotRow))
                    {
                        newProgram[i][j] = (program[i][j]) - ((program[i][pivotColumn]) * (newProgram[pivotRow][j]));
                    }
                }
            }

            // Update the program with the new pivoted values
           return newProgram;
       
        }

        #endregion


    }
}

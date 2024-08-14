using System;
using System.Collections.Generic;

namespace LPR_381_Project
{
    public class LinearProgramModel
    {
        public bool IsMaximization { get; set; }
        public List<double> ObjectiveCoefficients { get; set; }
        public List<Constraint> Constraints { get; set; }
        public List<string> SignRestrictions { get; set; } 

        public LinearProgramModel()
        {
            ObjectiveCoefficients = new List<double>();
            Constraints = new List<Constraint>();
            SignRestrictions = new List<string>();
        }

        public List<double> GetObjectiveCoefficients()
        {
            return new List<double>(ObjectiveCoefficients);
        }

        public List<double> GetConstraintCoefficients(int constraintIndex)
        {
            if (constraintIndex < 0 || constraintIndex >= Constraints.Count)
                throw new ArgumentOutOfRangeException(nameof(constraintIndex), "Invalid constraint index.");

            return new List<double>(Constraints[constraintIndex].Coefficients);
        }

        public double GetRightHandSide(int constraintIndex)
        {
            if (constraintIndex < 0 || constraintIndex >= Constraints.Count)
                throw new ArgumentOutOfRangeException(nameof(constraintIndex), "Invalid constraint index.");

            return Constraints[constraintIndex].RightHandSide;
        }

        public List<string> GetSignRestrictions()
        {
            return new List<string>(SignRestrictions);
        }

        public List<double> GetShadowPrices()
        {
            // Return an empty list or placeholder values
            return new List<double>();
        }
    }

    public class Constraint
    {
        public List<double> Coefficients { get; set; }
        public string Relation { get; set; }
        public double RightHandSide { get; set; }

        public Constraint()
        {
            Coefficients = new List<double>();
        }
    }
}
